
namespace DataLayer;

public static class RoleSeed
{
	public static List<Role> All =>
	[
		new()
		{
			id = 1,
			title = "Admin",
			created_at = new DateTime(2025, 1, 1, 12, 0, 0),
			updated_at = DateTime.Now,
			slug = "Admin_Role"
		},
		 new()
		{
			id = 2,
			title = "Customer",
			created_at = new DateTime(2025, 1, 1, 12, 0, 0),
			updated_at = new DateTime(2025, 1, 1, 12, 0, 0),
			slug = "Customer_Role"
		},
	];
}