using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer;
public interface ITicketRepository : IRepository<Ticket>
{
	Task<Ticket?> Get(string slug);
	Task<Ticket?> Get(long id);
	PaginatedList<Ticket> GetPaginated(DefaultPaginationFilter filter, long? user_id);
	Task<List<Ticket>> GetAll();
}
public class TicketRepository : Repository<Ticket>, ITicketRepository
{
	private readonly IQueryable<Ticket> _queryable;


	public TicketRepository(ApplicationDbContext context) : base(context)
	{
		_queryable = DbContext.Set<Ticket>();
	}

	public async Task<Ticket?> Get(string slug)
	{
		try
		{
			return await _queryable.AsNoTracking().Include(x => x.replies).SingleOrDefaultAsync(x => x.slug == slug);
		}
		catch
		{
			return null;
		}
	}

	public async Task<Ticket?> Get(long id)
	{
		try
		{
			return await _queryable.Include(x => x.replies).SingleOrDefaultAsync(x => x.id == id);
		}
		catch
		{
			return null;
		}
	}

	public async Task<List<Ticket>> GetAll()
	{
		try
		{
			return await _queryable.Include(x => x.replies).ToListAsync();
		}
		catch
		{
			return [];
		}
	}

	public PaginatedList<Ticket> GetPaginated(DefaultPaginationFilter filter, long? user_id)
	{
		try
		{
			var query = _queryable;
			if (user_id != null)
				query = query.Where(x=>x.user_id == user_id);
			query = query.Include(x => x.replies).AsNoTracking().Skip((filter.Page - 1) * filter.PageSize)
						.Take(filter.PageSize)
						.ApplyFilter(filter).ApplySort(filter.SortBy);
			var dataTotalCount = _queryable.Count();
			return new PaginatedList<Ticket>([.. query], dataTotalCount, filter.Page, filter.PageSize);
		}
		catch
		{
			return new PaginatedList<Ticket>([], 0, filter.Page, filter.PageSize);
		}
	}
}