using Microsoft.EntityFrameworkCore;

namespace DataLayer;

public interface IRolePermissionRepository : IRepository<RolePermission>
{
    Task<RolePermission?> Get(string slug);
    Task<RolePermission?> Get(long id);
    Task<List<RolePermission>> GetAllByRoleId(long roleId);
    PaginatedList<RolePermission> GetPaginated(DefaultPaginationFilter filter);
    Task<List<RolePermission>> GetAll();
}
public class RolePermissionRepository : Repository<RolePermission>, IRolePermissionRepository
{
    private readonly IQueryable<RolePermission> _queryable;


    public RolePermissionRepository(ApplicationDbContext context) : base(context)
    {
        _queryable = DbContext.Set<RolePermission>();
    }

    public async Task<RolePermission?> Get(string slug)
    {
        try
        {
            return await _queryable.AsNoTracking().SingleOrDefaultAsync(x => x.Slug == slug);
        }
        catch
        {
            return null;
        }
    }

    public async Task<RolePermission?> Get(long id)
    {
        try
        {
            return await _queryable.SingleOrDefaultAsync(x => x.Id == id);
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<RolePermission>> GetAll()
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

    public async Task<List<RolePermission>> GetAllByRoleId(long roleId)
    {
        try
        {
            return await _queryable.Where(rp => rp.RoleId == roleId).ToListAsync();
        }
        catch
        {
            return [];
        }
    }

    public PaginatedList<RolePermission> GetPaginated(DefaultPaginationFilter filter)
    {
        try
        {
            var query = _queryable.AsNoTracking().Skip((filter.Page - 1) * filter.PageSize)
                        .Take(filter.PageSize)
                        .ApplyFilter(filter).ApplySort(filter.SortBy);
            var dataTotalCount = _queryable.Count();
            return new PaginatedList<RolePermission>([.. query], dataTotalCount, filter.Page, filter.PageSize);
        }
        catch
        {
            return new PaginatedList<RolePermission>([], 0, filter.Page, filter.PageSize);
        }
    }
}