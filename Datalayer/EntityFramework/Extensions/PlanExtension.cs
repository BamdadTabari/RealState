using DataLayer.Assistant.Enums;

namespace DataLayer;
public static class PlanExtension
{
	public static IQueryable<Plan> ApplyFilter(this IQueryable<Plan> query, DefaultPaginationFilter filter)
	{

		if (!string.IsNullOrEmpty(filter.Keyword))
			query = query.Where(x => x.name.ToLower().Contains(filter.Keyword.ToLower().Trim())
			|| x.slug.ToLower().ToLower().Contains(filter.Keyword.ToLower().Trim())
			|| x.description.ToLower().ToLower().Contains(filter.Keyword.ToLower().Trim()));

		return query;
	}


	public static IQueryable<Plan> ApplySort(this IQueryable<Plan> query, SortByEnum? sortBy)
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