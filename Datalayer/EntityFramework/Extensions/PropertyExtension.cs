using DataLayer.Assistant.Enums;

namespace DataLayer;
public static class PropertyExtension
{
	public static IQueryable<Property> ApplyFilter(this IQueryable<Property> query, DefaultPaginationFilter filter)
	{

		if (!string.IsNullOrEmpty(filter.Keyword))
			query = query.Where(x => x.name.ToLower().Contains(filter.Keyword.ToLower().Trim())
			|| x.slug.ToLower().Contains(filter.Keyword.ToLower().Trim())
			|| x.code.ToLower().Contains(filter.Keyword.ToLower().Trim())
			|| x.city_province_full_name.ToLower().Contains(filter.Keyword.ToLower().Trim()));

		if(filter.BoolFilter != null)
			query =query.Where(x=>x.is_active == filter.BoolFilter);

		return query;
	}


	public static IQueryable<Property> ApplySort(this IQueryable<Property> query, SortByEnum? sortBy)
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