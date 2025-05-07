using Microsoft.EntityFrameworkCore;

namespace DataLayer;
public interface IPropertyFacilityPropertyRepository : IRepository<PropertyFacilityProperty>
{
	Task<PropertyFacilityProperty?> Get(string slug);
	Task<PropertyFacilityProperty?> Get(long id);
	//PaginatedList<PropertyFacilityProperty> GetPaginated(DefaultPaginationFilter filter);
	Task<List<PropertyFacilityProperty>> GetPropertyFacilityProperties(int count);
	Task<List<PropertyFacilityProperty>> GetAll();
	Task<List<PropertyFacilityProperty>> GetLatestPropertyFacilityProperties(int take);
}
public class PropertyFacilityPropertyRepository : Repository<PropertyFacilityProperty>, IPropertyFacilityPropertyRepository
{
	private readonly IQueryable<PropertyFacilityProperty> _queryable;


	public PropertyFacilityPropertyRepository(ApplicationDbContext context) : base(context)
	{
		_queryable = DbContext.Set<PropertyFacilityProperty>();
	}

	public async Task<PropertyFacilityProperty?> Get(string slug)
	{
		try
		{
			return await _queryable
				.Include(x=>x.property).Include(x=>x.property_facility)
				.AsNoTracking().SingleOrDefaultAsync(x => x.slug == slug);
		}
		catch
		{
			return null;
		}
	}

	public async Task<PropertyFacilityProperty?> Get(long id)
	{
		try
		{
			return await _queryable
				.Include(x => x.property).Include(x => x.property_facility)
				.SingleOrDefaultAsync(x => x.id == id);
		}
		catch
		{
			return null;
		}
	}

	public async Task<List<PropertyFacilityProperty>> GetAll()
	{
		try
		{
			return await _queryable.Include(x => x.property).Include(x => x.property_facility).ToListAsync();
		}
		catch
		{
			return [];
		}
	}

	public async Task<List<PropertyFacilityProperty>> GetPropertyFacilityProperties(int count)
	{
		return await _queryable.Include(x => x.property).Include(x => x.property_facility).AsNoTracking()
			.Skip(0)
			.Take(count)
			.ToListAsync();
	}

	public async Task<List<PropertyFacilityProperty>> GetLatestPropertyFacilityProperties(int take)
	{
		try
		{
			return await _queryable.Include(x => x.property).Include(x => x.property_facility).AsNoTracking().Skip(0).Take(take).ToListAsync();
		}
		catch
		{
			return [];
		}
	}

	//public PaginatedList<PropertyFacilityProperty> GetPaginated(DefaultPaginationFilter filter)
	//{
	//	try
	//	{
	//		var query = _queryable.AsNoTracking().Skip((filter.Page - 1) * filter.PageSize)
	//					.Take(filter.PageSize)
	//					.ApplyFilter(filter).ApplySort(filter.SortBy);
	//		var dataTotalCount = _queryable.Count();
	//		return new PaginatedList<PropertyFacilityProperty>([.. query], dataTotalCount, filter.Page, filter.PageSize);
	//	}
	//	catch
	//	{
	//		return new PaginatedList<PropertyFacilityProperty>([], 0, filter.Page, filter.PageSize);
	//	}
	//}
}
