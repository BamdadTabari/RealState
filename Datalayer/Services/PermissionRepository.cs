using Microsoft.EntityFrameworkCore;

namespace DataLayer;
public interface IPermissionRepository : IRepository<Permission>
{
    Task<Permission?> Get(string slug);
    Task<Permission?> Get(long id);
    PaginatedList<Permission> GetPaginated(DefaultPaginationFilter filter);
    Task<List<Permission>> GetAll();
}
public class PermissionRepository : Repository<Permission>, IPermissionRepository
{
    private readonly IQueryable<Permission> _queryable;


    public PermissionRepository(ApplicationDbContext context) : base(context)
    {
        _queryable = DbContext.Set<Permission>();
    }

    public async Task<Permission?> Get(string slug)
    {
        try
        {
            return await _queryable.Include(x => x.RolePermissions).AsNoTracking().SingleOrDefaultAsync(x => x.Slug == slug);
        }
        catch
        {
            return null;
        }
    }

    public async Task<Permission?> Get(long id)
    {
        try
        {
            return await _queryable.Include(x => x.RolePermissions).SingleOrDefaultAsync(x => x.Id == id);
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<Permission>> GetAll()
    {
        try
        {
            return await _queryable.Include(x => x.RolePermissions).ToListAsync();
        }
        catch
        {
            return [];
        }
    }

    public PaginatedList<Permission> GetPaginated(DefaultPaginationFilter filter)
    {
        try
        {
            var query = _queryable.AsNoTracking().Skip((filter.Page - 1) * filter.PageSize)
                        .Take(filter.PageSize)
                        .ApplyFilter(filter).ApplySort(filter.SortBy);
            var dataTotalCount = _queryable.Count();
            return new PaginatedList<Permission>([.. query], dataTotalCount, filter.Page, filter.PageSize);
        }
        catch
        {
            return new PaginatedList<Permission>([], 0, filter.Page, filter.PageSize);
        }
    }
}