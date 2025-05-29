using DataLayer.Services;

namespace DataLayer;
public interface IUnitOfWork : IDisposable
{
	IBlogCategoryRepository BlogCategoryRepository { get; }
	IBlogRepository BlogRepository { get; }
	IOrderRepository OrderRepository { get; }
	IOtpRepository OtpRepository { get; }
	ITokenBlacklistRepository TokenBlacklistRepository { get; }
	IUserRepo UserRepository { get; }
	IUserRoleRepo UserRoleRepository { get; }
	IRoleRepo RoleRepository { get; }
	ICityRepository CityRepository { get; }
	IOptionRepository OptionRepository { get; }
	IAgencyRepository AgencyRepository { get; }
	IPlanRepository PlanRepository { get; }
	IPropertyCategoryRepository PropertyCategoryRepository { get; }
	IPropertyFacilityRepository PropertyFacilityRepository { get; }
	IPropertyFacilityPropertyRepository PropertyFacilityPropertyRepository { get; }
	IPropertyRepository PropertyRepository { get; }
	IPropertySituationRepository PropertySituationRepository { get; }
	IProvinceRepository ProvinceRepository { get; }
	IPropertyGalleryRepository PropertyGalleryRepository { get; }
	ITicketRepository TicketRepository { get; }
	ITicketReplyRepository TicketReplyRepository { get; }
	IContactRepository	ContactRepository { get; }
	Task<bool> CommitAsync();
}

public class UnitOfWork : IUnitOfWork
{
	private readonly ApplicationDbContext _context;
	public UnitOfWork(ApplicationDbContext context)
	{
		_context = context;
		BlogCategoryRepository = new BlogCategoryRepository(_context);
		BlogRepository = new BlogRepository(_context);
		OrderRepository = new OrderRepository(_context);
		OtpRepository = new OtpRepository(_context);
		TokenBlacklistRepository = new TokenBlacklistRepo(_context);
		UserRepository = new UserRepo(_context);
		UserRoleRepository = new UserRoleRepo(_context);
		RoleRepository = new RoleRepo(_context);
		CityRepository = new CityRepository(_context);
		OptionRepository = new OptionRepository(_context);
		AgencyRepository = new AgencyRepository(_context);
		PlanRepository = new PlanRepository(_context);
		PropertyCategoryRepository = new PropertyCategoryRepository(_context);
		PropertyFacilityRepository = new PropertyFacilityRepository(_context);
		PropertyFacilityPropertyRepository = new PropertyFacilityPropertyRepository(_context);
		PropertyRepository = new PropertyRepository(_context);
		PropertySituationRepository = new PropertySituationRepository(_context);
		ProvinceRepository = new ProvinceRepository(_context);
		PropertyGalleryRepository = new PropertyGalleryRepository(_context);
		TicketRepository = new TicketRepository(_context);
		TicketReplyRepository = new TicketReplyRepository(_context);
		ContactRepository = new ContactRepository(_context);
	}

	public IBlogCategoryRepository BlogCategoryRepository { get; }
	public IBlogRepository BlogRepository { get; }
	public IOrderRepository OrderRepository { get; }
	public IOtpRepository OtpRepository { get; }
	public ITokenBlacklistRepository TokenBlacklistRepository { get; }
	public IUserRepo UserRepository { get; }
	public IUserRoleRepo UserRoleRepository { get; }
	public IRoleRepo RoleRepository { get; }
	public ICityRepository CityRepository { get; set; }
	public IOptionRepository OptionRepository { get; set; }
	public IAgencyRepository AgencyRepository { get; set; }
	public IPlanRepository PlanRepository { get; set; }
	public IPropertyCategoryRepository PropertyCategoryRepository { get; set; }
	public IPropertyFacilityRepository PropertyFacilityRepository { get; set; }
	public IPropertyFacilityPropertyRepository PropertyFacilityPropertyRepository { get; set; }
	public IPropertyRepository PropertyRepository { get; set; }
	public IPropertySituationRepository PropertySituationRepository { get; set; }
	public IProvinceRepository ProvinceRepository { get; set; }
	public IPropertyGalleryRepository PropertyGalleryRepository { get; set; }
	public ITicketRepository TicketRepository { get; set; }
	public ITicketReplyRepository TicketReplyRepository { get; set; }
	public IContactRepository ContactRepository { get; set; }
	public async Task<bool> CommitAsync() => await _context.SaveChangesAsync() > 0;

	// dispose and add to garbage collector
	public void Dispose()
	{
		_context.Dispose();
		GC.SuppressFinalize(this);
	}
}