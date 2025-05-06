using Microsoft.EntityFrameworkCore;

namespace DataLayer;

public interface IUserRoleRepo : IRepository<UserRole>
{
	IEnumerable<UserRole> GetUserRolesByUserId(long userId);
	Task<bool> HasPermissionAsync(long userId, string permissionName);
	//Task<PaginatedList<UserRole>> GetByRoleId(int roleId);
}
public class UserRoleRepo : Repository<UserRole>, IUserRoleRepo
{
	private readonly IQueryable<UserRole> _queryable;


	public UserRoleRepo(ApplicationDbContext context) : base(context)
	{
		_queryable = DbContext.Set<UserRole>();
	}

	//public Task<PaginatedList<UserRole>> GetByRoleId(int roleId)
	//{
	//    try
	//    {
	//        var query = _queryable.Where(x => x.Id == filter.LastId).AsNoTracking().ApplyFilter(filter).ApplySort(filter.SortBy);
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
			return _queryable.Include(i => i.Role).Where(x => x.UserId == userId);
		}
		catch
		{
			return new List<UserRole>().AsEnumerable();
		}
	}

	public async Task<bool> HasPermissionAsync(long userId, string permissionName)
	{
		return await _queryable
			.Where(ur => ur.UserId == userId)
			.SelectMany(ur => ur.Role.RolePermissions)
			.AnyAsync(rp => rp.Permission.Name == permissionName);
	}
}
