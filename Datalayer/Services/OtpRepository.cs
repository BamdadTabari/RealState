using Microsoft.EntityFrameworkCore;

namespace DataLayer;
public interface IOtpRepository : IRepository<Otp>
{
	Task<Otp> GetByPhone(string phone);
	Task<List<Otp>> GetAllByPhone(string phone);
}
public class OtpRepository : Repository<Otp>, IOtpRepository
{
	private readonly IQueryable<Otp> _queryable;


	public OtpRepository(ApplicationDbContext context) : base(context)
	{
		_queryable = DbContext.Set<Otp>();
	}

	public async Task<Otp> GetByPhone(string phone)
	{
		return await _queryable.FirstOrDefaultAsync(o => o.phone == phone && o.expiry_date > DateTime.Now);
	}

	public async Task<List<Otp>> GetAllByPhone(string phone)
	{
		var data = _queryable.Where(x => x.phone == phone);
		return await data.ToListAsync();
	}

}