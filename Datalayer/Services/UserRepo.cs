using Microsoft.EntityFrameworkCore;

namespace DataLayer;
public interface IUserRepo : IRepository<User>
{
	Task<User?> GetUser(string usernameOrEmail);
	Task<User?> Get(string slug);
	Task<List<User>> GetAll();
	Task<User?> GetUserByPhone(string phone);
	Task<User?> GetUser(long id);
	Task<bool> AnyExistUserName(string username);
	Task<bool> AnyExistEmail(string email);
	PaginatedList<User> GetPaginated(DefaultPaginationFilter filter);
}
public class UserRepo : Repository<User>, IUserRepo
{
	private readonly IQueryable<User> _queryable;


	public UserRepo(ApplicationDbContext context) : base(context)
	{
		_queryable = DbContext.Set<User>();
	}

	/// <summary>
	/// Check if user exist, in case not exist return false, in case query error return false, in case exist return true
	/// </summary>
	/// <param name="email"></param>
	/// <returns></returns>
	public async Task<bool> AnyExistEmail(string email)
	{
		try
		{
			return await _queryable.AnyAsync(x => x.email == email);
		}
		catch
		{
			return await Task.FromResult(false);
		}
	}

	/// <summary>
	/// Check if user exist, in case not exist return false, in case query error return false, in case exist return true
	/// </summary>
	/// <param name="username"></param>
	/// <returns></returns>
	public async Task<bool> AnyExistUserName(string username)
	{
		try
		{
			return await _queryable.AnyAsync(x => x.user_name == username);
		}
		catch
		{
			return await Task.FromResult(false);
		}
	}

	public async Task<User?> Get(string slug)
	{
		try
		{
			return await _queryable.SingleOrDefaultAsync(x => x.slug == slug);
		}
		catch
		{
			return null;
		}
	}

	public async Task<List<User>> GetAll()
	{
		return await _queryable.ToListAsync();
	}

	public PaginatedList<User> GetPaginated(DefaultPaginationFilter filter)
	{
		try
		{
			var query = _queryable.Where(x=>x.is_delete_able == true).Skip((filter.Page - 1) * filter.PageSize)
						.Take(filter.PageSize).AsNoTracking().ApplyFilter(filter).ApplySort(filter.SortBy);
			var dataTotalCount = _queryable.Count(x=>x.is_delete_able == true);
			return new PaginatedList<User>([.. query], dataTotalCount, filter.Page, filter.PageSize);
		}
		catch
		{
			return new PaginatedList<User>([], 0, filter.Page, filter.PageSize);
		}
	}

	/// <summary>
	/// Get User by username Or Email, in case not exist return new User, in query error return new User
	/// </summary>
	/// <param name="username"></param>
	/// <returns></returns>
	public async Task<User?> GetUser(string usernameOrEmail)
	{
		try
		{
			return await _queryable.FirstOrDefaultAsync(x => x.user_name == usernameOrEmail || x.email == usernameOrEmail);
		}
		catch
		{
			return null;
		}
	}

	public async Task<User?> GetUser(long id)
	{
		try
		{
			return await _queryable.Include(x=>x.agency).SingleOrDefaultAsync(x => x.id == id);
		}
		catch
		{
			return null;
		}
	}

	public async Task<User?> GetUserByPhone(string phone)
	{
		try
		{
			return await _queryable.FirstOrDefaultAsync(x => x.mobile == phone);
		}
		catch
		{
			return null;
		}
	}
}
