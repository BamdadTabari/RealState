using DataLayer.Assistant.Enums;

namespace DataLayer;
public static class BlogCategoryExtension
{
	public static IQueryable<BlogCategory> ApplyFilter(this IQueryable<BlogCategory> query, DefaultPaginationFilter filter)
	{

		if (!string.IsNullOrEmpty(filter.Keyword))
			query = query.Where(x => x.name.Contains(filter.Keyword.ToLower().Trim(), StringComparison.CurrentCultureIgnoreCase) || x.slug.ToLower().Contains(filter.Keyword.ToLower().Trim())
			|| x.description.Contains(filter.Keyword.ToLower().Trim(), StringComparison.CurrentCultureIgnoreCase));


		return query;
	}


	public static IQueryable<BlogCategory> ApplySort(this IQueryable<BlogCategory> query, SortByEnum? sortBy)
	{
		return sortBy switch
		{
			SortByEnum.CreationDate => query.OrderBy(x => x.created_at),
			SortByEnum.CreationDateDescending => query.OrderByDescending(x => x.created_at),
			SortByEnum.updated_ate => query.OrderBy(x => x.updated_at),
			SortByEnum.updated_ateDescending => query.OrderByDescending(x => x.updated_at),
			_ => query.OrderByDescending(x => x.id)
		};
	}
}
