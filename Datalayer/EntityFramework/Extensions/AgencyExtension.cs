using DataLayer.Assistant.Enums;

namespace DataLayer;
public static class AgencyExtension
{
	public static IQueryable<Agency> ApplyFilter(this IQueryable<Agency> query, DefaultPaginationFilter filter)
	{

		if (!string.IsNullOrEmpty(filter.Keyword))
			query = query.Where(x => x.full_name.ToLower().Contains(filter.Keyword.ToLower().Trim())
			|| x.slug.ToLower().Contains(filter.Keyword.ToLower().Trim())
			|| x.mobile.ToLower().Contains(filter.Keyword.ToLower().Trim())
			|| x.phone.ToLower().Contains(filter.Keyword.ToLower().Trim())
			|| x.city_province_full_name.ToLower().Contains(filter.Keyword.ToLower().Trim()));

		return query;
	}


	public static IQueryable<Agency> ApplySort(this IQueryable<Agency> query, SortByEnum? sortBy)
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