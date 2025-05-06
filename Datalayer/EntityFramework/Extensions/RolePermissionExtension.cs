using DataLayer.Assistant.Enums;

namespace DataLayer;
public static class RolePermissionExtension
{
    public static IQueryable<RolePermission> ApplyFilter(this IQueryable<RolePermission> query, DefaultPaginationFilter filter)
    {
        return query;
    }


    public static IQueryable<RolePermission> ApplySort(this IQueryable<RolePermission> query, SortByEnum? sortBy)
    {
        return sortBy switch
        {
            SortByEnum.CreationDate => query.OrderBy(x => x.CreatedAt),
            SortByEnum.CreationDateDescending => query.OrderByDescending(x => x.CreatedAt),
            SortByEnum.UpdateDate => query.OrderBy(x => x.UpdatedAt),
            SortByEnum.UpdateDateDescending => query.OrderByDescending(x => x.UpdatedAt),
            _ => query.OrderByDescending(x => x.Id)
        };
    }
}
