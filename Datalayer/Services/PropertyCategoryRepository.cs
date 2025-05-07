using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer;
public interface IPropertyCategoryRepository : IRepository<PropertyCategory>
{
	Task<PropertyCategory?> Get(string slug);
	Task<PropertyCategory?> Get(long id);
	PaginatedList<PropertyCategory> GetPaginated(DefaultPaginationFilter filter);
	Task<List<PropertyCategory>> GetPropertyCategories(int count);
	Task<List<PropertyCategory>> GetAll();
	Task<List<PropertyCategory>> GetLatestPropertyCategories(int take);
}
public class PropertyCategoryRepository : Repository<PropertyCategory>, IPropertyCategoryRepository
{
	private readonly IQueryable<PropertyCategory> _queryable;


	public PropertyCategoryRepository(ApplicationDbContext context) : base(context)
	{
		_queryable = DbContext.Set<PropertyCategory>();
	}

	public async Task<PropertyCategory?> Get(string slug)
	{
		try
		{
			return await _queryable.Include(x => x.properties).AsNoTracking().SingleOrDefaultAsync(x => x.slug == slug);
		}
		catch
		{
			return null;
		}
	}

	public async Task<PropertyCategory?> Get(long id)
	{
		try
		{
			return await _queryable.Include(x => x.properties).SingleOrDefaultAsync(x => x.id == id);
		}
		catch
		{
			return null;
		}
	}

	public async Task<List<PropertyCategory>> GetAll()
	{
		try
		{
			return await _queryable.Include(x => x.properties).ToListAsync();
		}
		catch
		{
			return [];
		}
	}

	public async Task<List<PropertyCategory>> GetPropertyCategories(int count)
	{
		return await _queryable.AsNoTracking()
			.Include(x => x.properties)
			.Skip(0)
			.Take(count)
			.ToListAsync();
	}

	public async Task<List<PropertyCategory>> GetLatestPropertyCategories(int take)
	{
		try
		{
			return await _queryable.Include(x => x.properties).AsNoTracking().Skip(0).Take(take).ToListAsync();
		}
		catch
		{
			return [];
		}
	}

	public PaginatedList<PropertyCategory> GetPaginated(DefaultPaginationFilter filter)
	{
		try
		{
			var query = _queryable.Include(x => x.properties).AsNoTracking().Skip((filter.Page - 1) * filter.PageSize)
						.Take(filter.PageSize)
						.ApplyFilter(filter).ApplySort(filter.SortBy);
			var dataTotalCount = _queryable.Count();
			return new PaginatedList<PropertyCategory>([.. query], dataTotalCount, filter.Page, filter.PageSize);
		}
		catch
		{
			return new PaginatedList<PropertyCategory>([], 0, filter.Page, filter.PageSize);
		}
	}
}