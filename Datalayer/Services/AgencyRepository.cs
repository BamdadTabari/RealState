using Microsoft.EntityFrameworkCore;

namespace DataLayer;
public interface IAgencyRepository : IRepository<Agency>
{
	Task<Agency?> Get(string slug);
	Task<Agency?> Get(long id);
	Task<Agency?> GetByUserId(long id);
	PaginatedList<Agency> GetPaginated(DefaultPaginationFilter filter);
	Task<List<Agency>> GetAgencies(int count);
	Task<List<Agency>> GetAll();
	Task<List<Agency>> GetLatestAgencies(int take);
}
public class AgencyRepository : Repository<Agency>, IAgencyRepository
{
	private readonly IQueryable<Agency> _queryable;


	public AgencyRepository(ApplicationDbContext context) : base(context)
	{
		_queryable = DbContext.Set<Agency>();
	}

	public async Task<Agency?> Get(string slug)
	{
		try
		{
			return await _queryable.Include(x => x.user).Include(x => x.city).AsNoTracking().SingleOrDefaultAsync(x => x.slug == slug);
		}
		catch
		{
			return null;
		}
	}

	public async Task<Agency?> Get(long id)
	{
		try
		{
			return await _queryable.Include(x => x.user).Include(x => x.city).SingleOrDefaultAsync(x => x.id == id);
		}
		catch
		{
			return null;
		}
	}

	public async Task<List<Agency>> GetAll()
	{
		try
		{
			return await _queryable.Include(x => x.user).Include(x => x.city).ToListAsync();
		}
		catch
		{
			return [];
		}
	}

	public async Task<List<Agency>> GetAgencies(int count)
	{
		return await _queryable.AsNoTracking()
			.Include(x => x.user).Include(x => x.city)
			.Skip(0)
			.Take(count)
			.ToListAsync();
	}

	public async Task<List<Agency>> GetLatestAgencies(int take)
	{
		try
		{
			return await _queryable.AsNoTracking().Include(x => x.user).Include(x => x.city).Skip(0).Take(take).ToListAsync();
		}
		catch
		{
			return [];
		}
	}

	public PaginatedList<Agency> GetPaginated(DefaultPaginationFilter filter)
	{
		try
		{
			var query = _queryable.Include(x => x.user).Include(x => x.city).AsNoTracking().Skip((filter.Page - 1) * filter.PageSize)
						.Take(filter.PageSize)
						.ApplyFilter(filter).ApplySort(filter.SortBy);
			var dataTotalCount = _queryable.Count();
			return new PaginatedList<Agency>([.. query], dataTotalCount, filter.Page, filter.PageSize);
		}
		catch
		{
			return new PaginatedList<Agency>([], 0, filter.Page, filter.PageSize);
		}
	}

	public async Task<Agency?> GetByUserId(long id)
	{
		try
		{
			return await _queryable.Include(x => x.user).Include(x => x.city).AsNoTracking().SingleOrDefaultAsync(x => x.user_id == id);
		}
		catch
		{
			return null;
		}
	}
}