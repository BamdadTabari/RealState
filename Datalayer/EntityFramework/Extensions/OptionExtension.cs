﻿using DataLayer.Assistant.Enums;

namespace DataLayer;
public static class OptionExtension
{
	public static IQueryable<Option> ApplyFilter(this IQueryable<Option> query, DefaultPaginationFilter filter)
	{

		if (!string.IsNullOrEmpty(filter.Keyword))
			query = query.Where(x => x.option_key.ToLower().Contains(filter.Keyword.ToLower().Trim()) ||
			x.option_value.ToLower().Contains(filter.Keyword.ToLower().Trim()) ||
			x.slug.ToLower().ToString().Contains(filter.Keyword.ToLower().Trim()));

		return query;
	}


	public static IQueryable<Option> ApplySort(this IQueryable<Option> query, SortByEnum? sortBy)
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