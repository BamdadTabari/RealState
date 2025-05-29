using DataLayer.Assistant.Enums;
using DataLayer.EntityFramework.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.EntityFramework.Extensions;
public static class ContactExtension
{
	public static IQueryable<Contact> ApplyFilter(this IQueryable<Contact> query, DefaultPaginationFilter filter)
	{

		if (!string.IsNullOrEmpty(filter.Keyword))
			query = query.Where(x => x.mobile.ToLower().ToLower().Contains(filter.Keyword.ToLower().Trim())
			|| x.slug.ToLower().ToLower().Contains(filter.Keyword.ToLower().Trim())
			|| x.message.ToLower().ToLower().Contains(filter.Keyword.ToLower().Trim())
			|| x.full_name.ToLower().ToLower().Contains(filter.Keyword.ToLower().Trim()));

		return query;
	}


	public static IQueryable<Contact> ApplySort(this IQueryable<Contact> query, SortByEnum? sortBy)
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