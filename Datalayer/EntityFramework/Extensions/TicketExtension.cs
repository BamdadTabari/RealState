using DataLayer.Assistant.Enums;

namespace DataLayer;
public static class TicketExtension
{
	public static IQueryable<Ticket> ApplyFilter(this IQueryable<Ticket> query, DefaultPaginationFilter filter)
	{

		if (!string.IsNullOrEmpty(filter.Keyword))
			query = query.Where(x => x.subject.ToLower().ToLower().Contains(filter.Keyword.ToLower().Trim())
			|| x.slug.ToLower().ToLower().Contains(filter.Keyword.ToLower().Trim())
			|| x.message.ToLower().ToLower().Contains(filter.Keyword.ToLower().Trim())
			|| x.ticket_code.ToLower().ToLower().Contains(filter.Keyword.ToLower().Trim()));

		if (filter.TicketStatus != null)
			query = query.Where(x => x.status == filter.TicketStatus);

		return query;
	}


	public static IQueryable<Ticket> ApplySort(this IQueryable<Ticket> query, SortByEnum? sortBy)
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