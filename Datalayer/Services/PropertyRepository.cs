using System.Security.Cryptography.X509Certificates;
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
	Task<List<Property>> GetSimilarProperties(int take, long categoryId, long cityId, TypeEnum typeEnum, long propertyId);
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
			.Include(x => x.gallery)
			.Include(x => x.property_category)
			.Include(x => x.property_facility_properties)
				.ThenInclude(pfp => pfp.property_facility)
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
			.Include(x => x.gallery)
			.Include(x => x.property_category)
			.Include(x => x.property_facility_properties)
				.ThenInclude(pfp => pfp.property_facility)
			.Include(x => x.situation)
			.SingleOrDefaultAsync(x => x.id == id);
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
				.ThenInclude(pfp => pfp.property_facility)
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
				.ThenInclude(pfp => pfp.property_facility)
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
				.ThenInclude(pfp => pfp.property_facility)
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

			if (userId != null)
				query = query.Where(x => x.owner_id == userId);

			query = query
				.Include(x => x.gallery)
				.Include(x => x.property_category)
				.Include(x => x.property_facility_properties)
					.ThenInclude(pfp => pfp.property_facility)
				.Include(x => x.situation)
				.AsNoTracking()
				.Skip((filter.Page - 1) * filter.PageSize)
				.Take(filter.PageSize)
				.ApplyFilter(filter)
				.ApplySort(filter.SortBy);

			var dataTotalCount = _queryable.Count();

			return new PaginatedList<Property>([.. query], dataTotalCount, filter.Page, filter.PageSize);
		}
		catch
		{
			return new PaginatedList<Property>([], 0, filter.Page, filter.PageSize);
		}
	}

	public Task<List<Property>> GetSimilarProperties(int take, long categoryId, long cityId, TypeEnum typeEnum, long propertyId)
	{
		return _queryable.Include(x => x.gallery)
			.Where(x=>x.category_id == categoryId && x.city_id == cityId && x.type_enum == typeEnum && x.id != propertyId)
			.Take(take).ToListAsync();
	}
}