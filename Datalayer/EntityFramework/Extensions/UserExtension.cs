using DataLayer.Assistant.Enums;

namespace DataLayer;
public static class UserExtension
{
	public static IQueryable<User> ApplyFilter(this IQueryable<User> query, DefaultPaginationFilter filter)
	{

		if (!string.IsNullOrEmpty(filter.Keyword))
			query = query.Where(x => x.email.ToString().ToLower().Contains(filter.Keyword.ToLower().Trim()) ||
			x.slug.ToLower().Contains(filter.Keyword.ToLower().Trim())
			|| x.mobile.ToLower().Contains(filter.Keyword.ToLower().Trim())
			|| x.user_name.ToLower().Contains(filter.Keyword.ToLower().Trim()));

		return query;
	}


	public static IQueryable<User> ApplySort(this IQueryable<User> query, SortByEnum? sortBy)
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