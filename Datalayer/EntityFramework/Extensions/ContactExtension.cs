using DataLayer.Assistant.Enums;

namespace DataLayer;
public static class ContactExtension
{
	public static IQueryable<Contact> ApplyFilter(this IQueryable<Contact> query, DefaultPaginationFilter filter)
	{

		if (!string.IsNullOrEmpty(filter.Keyword))
			query = query.Where(x => x.FullName.Contains(filter.Keyword.ToLower().Trim(), StringComparison.CurrentCultureIgnoreCase) ||
			x.Phone.Contains(filter.Keyword.ToLower().Trim(), StringComparison.CurrentCultureIgnoreCase)
			|| x.Email.Contains(filter.Keyword.ToLower().Trim(), StringComparison.CurrentCultureIgnoreCase) ||
			x.Slug.Contains(filter.Keyword.ToLower().Trim(), StringComparison.CurrentCultureIgnoreCase));

		return query;
	}


	public static IQueryable<Contact> ApplySort(this IQueryable<Contact> query, SortByEnum? sortBy)
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