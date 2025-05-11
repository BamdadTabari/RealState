using DataLayer.Assistant.Enums;

namespace DataLayer;

public static class BlogExtension
{
	public static IQueryable<Blog> ApplyFilter(this IQueryable<Blog> query, DefaultPaginationFilter filter)
	{

		if (!string.IsNullOrEmpty(filter.Keyword))
			query = query.Where(x => x.name.Contains(filter.Keyword.ToLower().Trim(), StringComparison.CurrentCultureIgnoreCase) ||
			x.slug.Contains(filter.Keyword.ToLower().Trim(), StringComparison.CurrentCultureIgnoreCase)
			|| x.description.Contains(filter.Keyword.ToLower().Trim(), StringComparison.CurrentCultureIgnoreCase));

		if (filter.BoolFilter != null)
			query = query.Where(x => x.show_blog == filter.BoolFilter);

		return query;
	}


	public static IQueryable<Blog> ApplySort(this IQueryable<Blog> query, SortByEnum? sortBy)
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
