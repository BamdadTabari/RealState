
using Microsoft.EntityFrameworkCore;

namespace DataLayer;
public interface IRoleRepo : IRepository<Role>
{
	Task<Role> GetRole(string title);
	Task<Role> GetRole(long id);
	Task<Role> GetRoleBySlug(string slug);
	Task<bool> AnyExist(string title);
	Task<List<Role>> GetAll();
	PaginatedList<Role> GetPaginated(DefaultPaginationFilter filter);
}
public class RoleRepo : Repository<Role>, IRoleRepo
{
	private readonly IQueryable<Role> _queryable;


	public RoleRepo(ApplicationDbContext context) : base(context)
	{
		_queryable = DbContext.Set<Role>();
	}

	public async Task<bool> AnyExist(string title)
	{
		try
		{
			return await _queryable.AnyAsync(x => x.title == title);
		}
		catch
		{
			return await Task.FromResult(false);
		}
	}

	public async Task<List<Role>> GetAll()
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

	public PaginatedList<Role> GetPaginated(DefaultPaginationFilter filter)
	{
		try
		{
			var query = _queryable.Skip((filter.Page - 1) * filter.PageSize)
						.Take(filter.PageSize).AsNoTracking().ApplyFilter(filter).ApplySort(filter.SortBy);
			var dataTotalCount = _queryable.Count();
			return new PaginatedList<Role>([.. query], dataTotalCount, filter.Page, filter.PageSize);
		}
		catch
		{
			return new PaginatedList<Role>([], 0, filter.Page, filter.PageSize);
		}
	}

	public async Task<Role> GetRole(string title)
	{
		try
		{
			return await _queryable.SingleOrDefaultAsync(x => x.title == title) ?? new Role();
		}
		catch
		{
			return new Role();
		}
	}
	public async Task<Role> GetRole(long id)
	{
		try
		{
			return await _queryable.SingleOrDefaultAsync(x => x.id == id) ?? new Role();
		}
		catch
		{
			return new Role();
		}
	}

	public async Task<Role> GetRoleBySlug(string slug)
	{
		try
		{
			return await _queryable.SingleOrDefaultAsync(x => x.slug == slug) ?? new Role();
		}
		catch
		{
			return new Role();
		}
	}
}
