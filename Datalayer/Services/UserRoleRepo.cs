using Microsoft.EntityFrameworkCore;

namespace DataLayer;

public interface IUserRoleRepo : IRepository<UserRole>
{
	IEnumerable<UserRole> GetUserRolesByUserId(long userId);
	//Task<PaginatedList<UserRole>> GetByRoleid(int roleid);
}
public class UserRoleRepo : Repository<UserRole>, IUserRoleRepo
{
	private readonly IQueryable<UserRole> _queryable;


	public UserRoleRepo(ApplicationDbContext context) : base(context)
	{
		_queryable = DbContext.Set<UserRole>();
	}

	//public Task<PaginatedList<UserRole>> GetByRoleid(int roleid)
	//{
	//    try
	//    {
	//        var query = _queryable.Where(x => x.id == filter.Lastid).AsNoTracking().ApplyFilter(filter).ApplySort(filter.SortBy);
	//        var dataTotalCount = _queryable.Count();
	//        return new PaginatedList<PostComment>([.. query], dataTotalCount, filter.Page, filter.PageSize);
	//    }
	//    catch
	//    {
	//        _logger.Error("Error in GetPaginatedPostComment");
	//        return new PaginatedList<PostComment>(new List<PostComment>(), 0, filter.Page, filter.PageSize);
	//    }
	//}

	public IEnumerable<UserRole> GetUserRolesByUserId(long userId)
	{
		try
		{
			return _queryable.Include(i => i.role).Where(x => x.user_id == userid);
		}
		catch
		{
			return new List<UserRole>().AsEnumerable();
		}
	}
}
