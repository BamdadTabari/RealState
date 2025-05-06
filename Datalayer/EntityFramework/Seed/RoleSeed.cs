
namespace DataLayer;

public static class RoleSeed
{
	public static List<Role> All =>
	[
		new()
		{
			id = 1,
			Title = "Admin",
			created_at = new DateTime(2025, 1, 1, 12, 0, 0),
			updated_at = DateTime.Now,
			slug = "Admin_Role"
		},
		 new()
		{
			id = 2,
			Title = "Customer",
			created_at = new DateTime(2025, 1, 1, 12, 0, 0),
			updated_at = new DateTime(2025, 1, 1, 12, 0, 0),
			slug = "Customer_Role"
		},
		 new()
		{
			id = 3,
			Title = "SaleMan",
			created_at = new DateTime(2025, 1, 1, 12, 0, 0),
			updated_at = new DateTime(2025, 1, 1, 12, 0, 0),
			slug = "SaleMan_Role"
		}
	];
}