using DataLayer.Assistant.Enums;

namespace DataLayer;
public static class BlogCategoryExtension
{
	public static IQueryable<BlogCategory> ApplyFilter(this IQueryable<BlogCategory> query, DefaultPaginationFilter filter)
	{

		if (!string.IsNullOrEmpty(filter.Keyword))
			query = query.Where(x => x.Name.Contains(filter.Keyword.ToLower().Trim(), StringComparison.CurrentCultureIgnoreCase) || x.Slug.ToLower().Contains(filter.Keyword.ToLower().Trim())
			|| x.Description.Contains(filter.Keyword.ToLower().Trim(), StringComparison.CurrentCultureIgnoreCase));


		return query;
	}


	public static IQueryable<BlogCategory> ApplySort(this IQueryable<BlogCategory> query, SortByEnum? sortBy)
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
