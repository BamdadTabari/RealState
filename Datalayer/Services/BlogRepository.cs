﻿using Microsoft.EntityFrameworkCore;

namespace DataLayer;

public interface IBlogRepository : IRepository<Blog>
{
	Task<Blog?> Get(string slug);
	Task<Blog?> Get(long id);
	PaginatedList<Blog> GetPaginated(DefaultPaginationFilter filter, long? categoryId);
	Task<List<Blog>> GetBlogs(int count);
	Task<List<Blog>> GetAll();
	Task<List<Blog>> GetLatestBlogs(int take);
}
public class BlogRepository : Repository<Blog>, IBlogRepository
{
	private readonly IQueryable<Blog> _queryable;


	public BlogRepository(ApplicationDbContext context) : base(context)
	{
		_queryable = DbContext.Set<Blog>();
	}

	public async Task<Blog?> Get(string slug)
	{
		try
		{
			return await _queryable.Include(x => x.blog_category).AsNoTracking().SingleOrDefaultAsync(x => x.slug == slug);
		}
		catch
		{
			return null;
		}
	}

	public async Task<Blog?> Get(long id)
	{
		try
		{
			return await _queryable.Include(x => x.blog_category).SingleOrDefaultAsync(x => x.id == id);
		}
		catch
		{
			return null;
		}
	}

	public async Task<List<Blog>> GetAll()
	{
		try
		{
			return await _queryable.Include(x => x.blog_category).ToListAsync();
		}
		catch
		{
			return [];
		}
	}

	public async Task<List<Blog>> GetBlogs(int count)
	{
		return await _queryable.AsNoTracking()
			.Include(x => x.blog_category)
			.Where(x => x.show_blog)
			.Skip(0)
			.Take(count)
			.ToListAsync();
	}

	public async Task<List<Blog>> GetLatestBlogs(int take)
	{
		try
		{
			return await _queryable.AsNoTracking().Where(x => x.show_blog).Skip(0).Take(take).ToListAsync();
		}
		catch
		{
			return [];
		}
	}

	public PaginatedList<Blog> GetPaginated(DefaultPaginationFilter filter, long? categoryId)
	{
		try
		{
			IQueryable<Blog> query = _queryable;
			if (categoryId != null)
				query = query.Include(x => x.blog_category).Where(x => x.blog_category_id == categoryId);
			query = query.Include(x => x.blog_category).AsNoTracking().Skip((filter.Page - 1) * filter.PageSize)
						.Take(filter.PageSize)
						.ApplyFilter(filter).ApplySort(filter.SortBy);
			var dataTotalCount = _queryable.Count();
			return new PaginatedList<Blog>([.. query], dataTotalCount, filter.Page, filter.PageSize);
		}
		catch
		{
			return new PaginatedList<Blog>([], 0, filter.Page, filter.PageSize);
		}
	}
}