using Microsoft.EntityFrameworkCore;

namespace DataLayer;
public interface IPropertyGalleryRepository : IRepository<PropertyGallery>
{
	Task<PropertyGallery?> Get(string slug);
	Task<PropertyGallery?> Get(long id);
	Task<List<PropertyGallery>> GetAll();
}
public class PropertyGalleryRepository : Repository<PropertyGallery>, IPropertyGalleryRepository
{
	private readonly IQueryable<PropertyGallery> _queryable;


	public PropertyGalleryRepository(ApplicationDbContext context) : base(context)
	{
		_queryable = DbContext.Set<PropertyGallery>();
	}

	public async Task<PropertyGallery?> Get(string slug)
	{
		try
		{
			return await _queryable.AsNoTracking().Include(x => x.property).SingleOrDefaultAsync(x => x.slug == slug);
		}
		catch
		{
			return null;
		}
	}

	public async Task<PropertyGallery?> Get(long id)
	{
		try
		{
			return await _queryable.Include(x => x.property).SingleOrDefaultAsync(x => x.id == id);
		}
		catch
		{
			return null;
		}
	}

	public async Task<List<PropertyGallery>> GetAll()
	{
		try
		{
			return await _queryable.Include(x => x.property).ToListAsync();
		}
		catch
		{
			return [];
		}
	}
}