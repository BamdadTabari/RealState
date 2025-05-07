using Microsoft.EntityFrameworkCore;

namespace DataLayer;
public interface IPropertySituationRepository : IRepository<PropertySituation>
{
	Task<PropertySituation?> Get(string slug);
	Task<PropertySituation?> Get(long id);
	PaginatedList<PropertySituation> GetPaginated(DefaultPaginationFilter filter);
	Task<List<PropertySituation>> GetPropertySituations(int count);
	Task<List<PropertySituation>> GetAll();
	Task<List<PropertySituation>> GetLatestPropertySituations(int take);
}
public class PropertySituationRepository : Repository<PropertySituation>, IPropertySituationRepository
{
	private readonly IQueryable<PropertySituation> _queryable;


	public PropertySituationRepository(ApplicationDbContext context) : base(context)
	{
		_queryable = DbContext.Set<PropertySituation>();
	}

	public async Task<PropertySituation?> Get(string slug)
	{
		try
		{
			return await _queryable.Include(x=>x.properties).AsNoTracking().SingleOrDefaultAsync(x => x.slug == slug);
		}
		catch
		{
			return null;
		}
	}

	public async Task<PropertySituation?> Get(long id)
	{
		try
		{
			return await _queryable.Include(x => x.properties).SingleOrDefaultAsync(x => x.id == id);
		}
		catch
		{
			return null;
		}
	}

	public async Task<List<PropertySituation>> GetAll()
	{
		try
		{
			return await _queryable.Include(x => x.properties).ToListAsync();
		}
		catch
		{
			return [];
		}
	}

	public async Task<List<PropertySituation>> GetPropertySituations(int count)
	{
		return await _queryable.Include(x => x.properties).AsNoTracking()
			.Skip(0)
			.Take(count)
			.ToListAsync();
	}

	public async Task<List<PropertySituation>> GetLatestPropertySituations(int take)
	{
		try
		{
			return await _queryable.Include(x => x.properties).AsNoTracking().Skip(0).Take(take).ToListAsync();
		}
		catch
		{
			return [];
		}
	}

	public PaginatedList<PropertySituation> GetPaginated(DefaultPaginationFilter filter)
	{
		try
		{
			var query = _queryable.Include(x => x.properties).AsNoTracking().Skip((filter.Page - 1) * filter.PageSize)
						.Take(filter.PageSize)
						.ApplyFilter(filter).ApplySort(filter.SortBy);
			var dataTotalCount = _queryable.Count();
			return new PaginatedList<PropertySituation>([.. query], dataTotalCount, filter.Page, filter.PageSize);
		}
		catch
		{
			return new PaginatedList<PropertySituation>([], 0, filter.Page, filter.PageSize);
		}
	}
}