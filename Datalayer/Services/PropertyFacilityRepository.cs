using Microsoft.EntityFrameworkCore;

namespace DataLayer;

public interface IPropertyFacilityRepository : IRepository<PropertyFacility>
{
	Task<PropertyFacility?> Get(string slug);
	Task<PropertyFacility?> Get(long id);
	PaginatedList<PropertyFacility> GetPaginated(DefaultPaginationFilter filter);
	Task<List<PropertyFacility>> GetPropertyFacilities(int count);
	Task<List<PropertyFacility>> GetAll();
	Task<List<PropertyFacility>> GetLatestPropertyFacilities(int take);
}
public class PropertyFacilityRepository : Repository<PropertyFacility>, IPropertyFacilityRepository
{
	private readonly IQueryable<PropertyFacility> _queryable;


	public PropertyFacilityRepository(ApplicationDbContext context) : base(context)
	{
		_queryable = DbContext.Set<PropertyFacility>();
	}

	public async Task<PropertyFacility?> Get(string slug)
	{
		try
		{
			return await _queryable
				.Include(x=>x.property_facility_properties)
				.ThenInclude(x=>x.property)
				.AsNoTracking().SingleOrDefaultAsync(x => x.slug == slug);
		}
		catch
		{
			return null;
		}
	}

	public async Task<PropertyFacility?> Get(long id)
	{
		try
		{
			return await _queryable
				.Include(x => x.property_facility_properties)
				.ThenInclude(x => x.property)
				.SingleOrDefaultAsync(x => x.id == id);
		}
		catch
		{
			return null;
		}
	}

	public async Task<List<PropertyFacility>> GetAll()
	{
		try
		{
			return await _queryable.Include(x => x.property_facility_properties).ThenInclude(x => x.property).ToListAsync();
		}
		catch
		{
			return [];
		}
	}

	public async Task<List<PropertyFacility>> GetPropertyFacilities(int count)
	{
		return await _queryable.Include(x => x.property_facility_properties).ThenInclude(x => x.property).AsNoTracking()
			.Skip(0)
			.Take(count)
			.ToListAsync();
	}

	public async Task<List<PropertyFacility>> GetLatestPropertyFacilities(int take)
	{
		try
		{
			return await _queryable.Include(x => x.property_facility_properties).ThenInclude(x => x.property).AsNoTracking().Skip(0).Take(take).ToListAsync();
		}
		catch
		{
			return [];
		}
	}

	public PaginatedList<PropertyFacility> GetPaginated(DefaultPaginationFilter filter)
	{
		try
		{
			var query = _queryable.Include(x => x.property_facility_properties).ThenInclude(x => x.property)
						.AsNoTracking().Skip((filter.Page - 1) * filter.PageSize)
						.Take(filter.PageSize)
						.ApplyFilter(filter).ApplySort(filter.SortBy);
			var dataTotalCount = _queryable.Count();
			return new PaginatedList<PropertyFacility>([.. query], dataTotalCount, filter.Page, filter.PageSize);
		}
		catch
		{
			return new PaginatedList<PropertyFacility>([], 0, filter.Page, filter.PageSize);
		}
	}
}