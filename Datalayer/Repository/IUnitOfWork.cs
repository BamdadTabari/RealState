namespace DataLayer;
public interface IUnitOfWork : IDisposable
{
    IBlogCategoryRepository BlogCategoryRepository { get; }
    IBlogRepository BlogRepository { get; }
    IOrderRepository OrderRepository { get; }
    IContactRepository ContactRepository { get; }
    IOtpRepository OtpRepository { get; }
    ITokenBlacklistRepository TokenBlacklistRepository { get; }
    IUserRepo UserRepository { get; }
    IUserRoleRepo UserRoleRepository { get; }
    IRoleRepo RoleRepository { get; }
    IPermissionRepository PermissionRepository { get; }
    IRolePermissionRepository RolePermissionRepository { get; }
    ICityRepository CityRepository { get; }
    IOptionRepository OptionRepository { get; }

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
        ContactRepository = new ContactRepository(_context);
        OtpRepository = new OtpRepository(_context);
        TokenBlacklistRepository = new TokenBlacklistRepo(_context);
        UserRepository = new UserRepo(_context);
        UserRoleRepository = new UserRoleRepo(_context);
        RoleRepository = new RoleRepo(_context);
        PermissionRepository = new PermissionRepository(_context);
        RolePermissionRepository = new RolePermissionRepository(_context);
        CityRepository = new CityRepository(_context);
        OptionRepository = new OptionRepository(_context);

    }

    public IBlogCategoryRepository BlogCategoryRepository { get; }
    public IBlogRepository BlogRepository { get; }
    public IOrderRepository OrderRepository { get; }
    public IContactRepository ContactRepository { get; }
    public IOtpRepository OtpRepository { get; }
    public ITokenBlacklistRepository TokenBlacklistRepository { get; }
    public IUserRepo UserRepository { get; }
    public IUserRoleRepo UserRoleRepository { get; }
    public IRoleRepo RoleRepository { get; }
    public IPermissionRepository PermissionRepository { get; set; }
    public IRolePermissionRepository RolePermissionRepository { get; set; }
    public ICityRepository CityRepository { get; set; }
    public IOptionRepository OptionRepository { get; set; }
    public async Task<bool> CommitAsync() => await _context.SaveChangesAsync() > 0;

    // dispose and add to garbage collector
    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}