using Microsoft.EntityFrameworkCore;

namespace DataLayer;

public interface IUserRoleRepo : IRepository<UserRole>
{
	IEnumerable<UserRole> GetUserRolesByUserid(long userid);
	Task<bool> HasPermissionAsync(long userid, string permissionName);
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

	public IEnumerable<UserRole> GetUserRolesByUserid(long userid)
	{
		try
		{
			return _queryable.Include(i => i.Role).Where(x => x.Userid == userid);
		}
		catch
		{
			return new List<UserRole>().AsEnumerable();
		}
	}

	public async Task<bool> HasPermissionAsync(long userid, string permissionName)
	{
		return await _queryable
			.Where(ur => ur.Userid == userid)
			.SelectMany(ur => ur.Role.RolePermissions)
			.AnyAsync(rp => rp.Permission.Name == permissionName);
	}
}
