using Microsoft.EntityFrameworkCore;

namespace DataLayer;
public interface IPropertyRepository : IRepository<Property>
{
	Task<Property?> Get(string slug);
	Task<Property?> Get(long id);
	PaginatedList<Property> GetPaginated(DefaultPaginationFilter filter, long? userId);
	Task<List<Property>> GetProperties(int count);
	Task<List<Property>> GetAll();
	Task<List<Property>> GetLatestProperties(int take);
}
public class PropertyRepository : Repository<Property>, IPropertyRepository
{
	private readonly IQueryable<Property> _queryable;


	public PropertyRepository(ApplicationDbContext context) : base(context)
	{
		_queryable = DbContext.Set<Property>();
	}

	public async Task<Property?> Get(string slug)
	{
		try
		{
			return await _queryable.AsNoTracking()
				.Include(x=>x.gallery)
				.Include(x => x.property_category)
				.Include(x => x.property_facility_properties)
				.ThenInclude(x=> x.Select(x=>x.property_facility))
				.Include(x => x.situation)
				.SingleOrDefaultAsync(x => x.slug == slug);
		}
		catch
		{
			return null;
		}
	}

	public async Task<Property?> Get(long id)
	{
		try
		{
			return await _queryable
				.Include(x=>x.user)
				.Include(x => x.gallery)
				.Include(x => x.property_category)
				.Include(x => x.property_facility_properties)
				.ThenInclude(x => x.Select(x => x.property_facility))
				.Include(x => x.situation).SingleOrDefaultAsync(x => x.id == id);
		}
		catch
		{
			return null;
		}
	}

	public async Task<List<Property>> GetAll()
	{
		try
		{
			return await _queryable
				.Include(x => x.gallery)
				.Include(x => x.property_category)
				.Include(x => x.property_facility_properties)
				.ThenInclude(x => x.Select(x => x.property_facility))
				.Include(x => x.situation).ToListAsync();
		}
		catch
		{
			return [];
		}
	}

	public async Task<List<Property>> GetProperties(int count)
	{
		return await _queryable.AsNoTracking()
			.Include(x => x.gallery)
			.Include(x => x.property_category)
			.Include(x => x.property_facility_properties)
			.ThenInclude(x => x.Select(x => x.property_facility))
			.Include(x => x.situation)
			.Skip(0)
			.Take(count)
			.ToListAsync();
	}

	public async Task<List<Property>> GetLatestProperties(int take)
	{
		try
		{
			return await _queryable.AsNoTracking()
				.Include(x => x.gallery)
				.Include(x => x.property_category)
				.Include(x => x.property_facility_properties)
				.ThenInclude(x => x.Select(x => x.property_facility))
				.Include(x => x.situation).Skip(0).Take(take).ToListAsync();
		}
		catch
		{
			return [];
		}
	}

	public PaginatedList<Property> GetPaginated(DefaultPaginationFilter filter, long? userId)
	{
		try
		{
			IQueryable<Property> query = _queryable;
			if(userId != null)
				query = query.Where(x=>x.owner_id == userId);
			query = query
				.Include(x => x.gallery)
				.Include(x => x.property_category)
				.Include(x => x.property_facility_properties)
				.ThenInclude(x => x.Select(x => x.property_facility))
				.Include(x => x.situation)
				.AsNoTracking().Skip((filter.Page - 1) * filter.PageSize)
						.Take(filter.PageSize)
						.ApplyFilter(filter).ApplySort(filter.SortBy);
			var dataTotalCount = _queryable.Count();
			return new PaginatedList<Property>([.. query], dataTotalCount, filter.Page, filter.PageSize);
		}
		catch
		{
			return new PaginatedList<Property>([], 0, filter.Page, filter.PageSize);
		}
	}
}