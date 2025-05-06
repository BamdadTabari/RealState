using DataLayer.Assistant.Enums;

namespace DataLayer;
public static class OptionExtension
{
	public static IQueryable<Option> ApplyFilter(this IQueryable<Option> query, DefaultPaginationFilter filter)
	{

		if (!string.IsNullOrEmpty(filter.Keyword))
			query = query.Where(x => x.OptionKey.Contains(filter.Keyword.ToLower().Trim(), StringComparison.CurrentCultureIgnoreCase) ||
			x.OptionValue.Contains(filter.Keyword.ToLower().Trim(), StringComparison.CurrentCultureIgnoreCase) ||
			x.Slug.ToString().Contains(filter.Keyword.ToLower().Trim(), StringComparison.CurrentCultureIgnoreCase));

		return query;
	}


	public static IQueryable<Option> ApplySort(this IQueryable<Option> query, SortByEnum? sortBy)
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