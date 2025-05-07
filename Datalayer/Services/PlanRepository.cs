
using Microsoft.EntityFrameworkCore;

namespace DataLayer;
public interface IPlanRepository : IRepository<Plan>
{
	Task<Plan?> Get(string slug);
	Task<Plan?> Get(long id);
	PaginatedList<Plan> GetPaginated(DefaultPaginationFilter filter);
	Task<List<Plan>> GetPlans(int count);
	Task<List<Plan>> GetAll();
	Task<List<Plan>> GetLatestPlans(int take);
}
public class PlanRepository : Repository<Plan>, IPlanRepository
{
	private readonly IQueryable<Plan> _queryable;


	public PlanRepository(ApplicationDbContext context) : base(context)
	{
		_queryable = DbContext.Set<Plan>();
	}

	public async Task<Plan?> Get(string slug)
	{
		try
		{
			return await _queryable.AsNoTracking().SingleOrDefaultAsync(x => x.slug == slug);
		}
		catch
		{
			return null;
		}
	}

	public async Task<Plan?> Get(long id)
	{
		try
		{
			return await _queryable.SingleOrDefaultAsync(x => x.id == id);
		}
		catch
		{
			return null;
		}
	}

	public async Task<List<Plan>> GetAll()
	{
		try
		{
			return await _queryable.ToListAsync();
		}
		catch
		{
			return [];
		}
	}

	public async Task<List<Plan>> GetPlans(int count)
	{
		return await _queryable.AsNoTracking()
			.Skip(0)
			.Take(count)
			.ToListAsync();
	}

	public async Task<List<Plan>> GetLatestPlans(int take)
	{
		try
		{
			return await _queryable.AsNoTracking().Skip(0).Take(take).ToListAsync();
		}
		catch
		{
			return [];
		}
	}

	public PaginatedList<Plan> GetPaginated(DefaultPaginationFilter filter)
	{
		try
		{
			var query = _queryable.AsNoTracking().Skip((filter.Page - 1) * filter.PageSize)
						.Take(filter.PageSize)
						.ApplyFilter(filter).ApplySort(filter.SortBy);
			var dataTotalCount = _queryable.Count();
			return new PaginatedList<Plan>([.. query], dataTotalCount, filter.Page, filter.PageSize);
		}
		catch
		{
			return new PaginatedList<Plan>([], 0, filter.Page, filter.PageSize);
		}
	}
}