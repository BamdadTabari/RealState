﻿using Microsoft.EntityFrameworkCore;

namespace DataLayer;
public interface ICityRepository : IRepository<City>
{
	Task<City?> Get(string slug);
	Task<City?> Get(long id);
	PaginatedList<City> GetPaginated(DefaultPaginationFilter filter);
	Task<List<City>> GetAll();
}
public class CityRepository : Repository<City>, ICityRepository
{
	private readonly IQueryable<City> _queryable;


	public CityRepository(ApplicationDbContext context) : base(context)
	{
		_queryable = DbContext.Set<City>();
	}

	public async Task<City?> Get(string slug)
	{
		try
		{
			return await _queryable.AsNoTracking().Include(x => x.province).Include(x=>x.agency_list).SingleOrDefaultAsync(x => x.slug == slug);
		}
		catch
		{
			return null;
		}
	}

	public async Task<City?> Get(long id)
	{
		try
		{
			return await _queryable.Include(x => x.province).Include(x => x.agency_list).SingleOrDefaultAsync(x => x.id == id);
		}
		catch
		{
			return null;
		}
	}

	public async Task<List<City>> GetAll()
	{
		try
		{
			return await _queryable.Include(x => x.province).Include(x => x.agency_list).ToListAsync();
		}
		catch
		{
			return [];
		}
	}

	public PaginatedList<City> GetPaginated(DefaultPaginationFilter filter)
	{
		try
		{
			var query = _queryable.Include(x => x.province).Include(x => x.agency_list).AsNoTracking().Skip((filter.Page - 1) * filter.PageSize)
						.Take(filter.PageSize)
						.ApplyFilter(filter).ApplySort(filter.SortBy);
			var dataTotalCount = _queryable.Count();
			return new PaginatedList<City>([.. query], dataTotalCount, filter.Page, filter.PageSize);
		}
		catch
		{
			return new PaginatedList<City>([], 0, filter.Page, filter.PageSize);
		}
	}
}