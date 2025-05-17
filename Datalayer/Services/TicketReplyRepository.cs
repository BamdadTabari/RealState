using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer;
public interface ITicketReplyRepository : IRepository<TicketReply>
{
	Task<TicketReply?> Get(string slug);
	Task<TicketReply?> Get(long id);
	Task<List<TicketReply>> GetAll();
}
public class TicketReplyRepository : Repository<TicketReply>, ITicketReplyRepository
{
	private readonly IQueryable<TicketReply> _queryable;


	public TicketReplyRepository(ApplicationDbContext context) : base(context)
	{
		_queryable = DbContext.Set<TicketReply>();
	}

	public async Task<TicketReply?> Get(string slug)
	{
		try
		{
			return await _queryable.AsNoTracking().Include(x => x.ticket).SingleOrDefaultAsync(x => x.slug == slug);
		}
		catch
		{
			return null;
		}
	}

	public async Task<TicketReply?> Get(long id)
	{
		try
		{
			return await _queryable.Include(x => x.ticket).SingleOrDefaultAsync(x => x.id == id);
		}
		catch
		{
			return null;
		}
	}

	public async Task<List<TicketReply>> GetAll()
	{
		try
		{
			return await _queryable.Include(x => x.ticket).ToListAsync();
		}
		catch
		{
			return [];
		}
	}

}