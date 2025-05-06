using Microsoft.EntityFrameworkCore;

namespace DataLayer;
public interface IContactRepository : IRepository<Contact>
{
	Task<Contact?> Get(string slug);
	Task<Contact?> Get(long id);
	PaginatedList<Contact> GetPaginated(DefaultPaginationFilter filter);
	Task<List<Contact>> GetAll();
}
public class ContactRepository : Repository<Contact>, IContactRepository
{
	private readonly IQueryable<Contact> _queryable;


	public ContactRepository(ApplicationDbContext context) : base(context)
	{
		_queryable = DbContext.Set<Contact>();
	}

	public async Task<Contact?> Get(string slug)
	{
		try
		{
			return await _queryable.AsNoTracking().SingleOrDefaultAsync(x => x.slug == slug);
		}
		catch
		{
			return null;
		}
	}

	public async Task<Contact?> Get(long id)
	{
		try
		{
			return await _queryable.SingleOrDefaultAsync(x => x.id == id);
		}
		catch
		{
			return null;
		}
	}

	public async Task<List<Contact>> GetAll()
	{
		try
		{
			return await _queryable.ToListAsync();
		}
		catch
		{
			return [];
		}
	}

	public PaginatedList<Contact> GetPaginated(DefaultPaginationFilter filter)
	{
		try
		{
			var query = _queryable.AsNoTracking().Skip((filter.Page - 1) * filter.PageSize)
						.Take(filter.PageSize)
						.ApplyFilter(filter).ApplySort(filter.SortBy);
			var dataTotalCount = _queryable.Count();
			return new PaginatedList<Contact>([.. query], dataTotalCount, filter.Page, filter.PageSize);
		}
		catch
		{
			return new PaginatedList<Contact>([], 0, filter.Page, filter.PageSize);
		}
	}
}