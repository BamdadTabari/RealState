using DataLayer.Assistant.Enums;

namespace DataLayer;
public static class PropertyFacilityExtension
{
	public static IQueryable<PropertyFacility> ApplyFilter(this IQueryable<PropertyFacility> query, DefaultPaginationFilter filter)
	{

		if (!string.IsNullOrEmpty(filter.Keyword))
			query = query.Where(x => x.name.ToLower().Contains(filter.Keyword.ToLower().Trim())
			|| x.slug.ToLower().Contains(filter.Keyword.ToLower().Trim()));

		return query;
	}


	public static IQueryable<PropertyFacility> ApplySort(this IQueryable<PropertyFacility> query, SortByEnum? sortBy)
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