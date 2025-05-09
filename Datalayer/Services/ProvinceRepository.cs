using Microsoft.EntityFrameworkCore;

namespace DataLayer;
public interface IProvinceRepository : IRepository<Province>
{
	Task<Province?> Get(string slug);
	Task<Province?> Get(long id);
	PaginatedList<Province> GetPaginated(DefaultPaginationFilter filter);
	Task<List<Province>> GetAll();
}
public class ProvinceRepository : Repository<Province>, IProvinceRepository
{
	private readonly IQueryable<Province> _queryable;


	public ProvinceRepository(ApplicationDbContext context) : base(context)
	{
		_queryable = DbContext.Set<Province>();
	}

	public async Task<Province?> Get(string slug)
	{
		try
		{
			return await _queryable.AsNoTracking().Include(x => x.cities).SingleOrDefaultAsync(x => x.slug == slug);
		}
		catch
		{
			return null;
		}
	}

	public async Task<Province?> Get(long id)
	{
		try
		{
			return await _queryable.Include(x => x.cities).SingleOrDefaultAsync(x => x.id == id);
		}
		catch
		{
			return null;
		}
	}

	public async Task<List<Province>> GetAll()
	{
		try
		{
			return await _queryable.Include(x => x.cities).ToListAsync();
		}
		catch
		{
			return [];
		}
	}

	public PaginatedList<Province> GetPaginated(DefaultPaginationFilter filter)
	{
		try
		{
			var query = _queryable.Include(x => x.cities).AsNoTracking().Skip((filter.Page - 1) * filter.PageSize)
						.Take(filter.PageSize)
						.ApplyFilter(filter).ApplySort(filter.SortBy);
			var dataTotalCount = _queryable.Count();
			return new PaginatedList<Province>([.. query], dataTotalCount, filter.Page, filter.PageSize);
		}
		catch
		{
			return new PaginatedList<Province>([], 0, filter.Page, filter.PageSize);
		}
	}
}