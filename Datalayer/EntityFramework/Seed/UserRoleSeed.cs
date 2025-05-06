namespace DataLayer;

public static class UserRoleSeed
{
	public static List<UserRole> All =>
	[
		new()
		{
			Roleid = 1,
			Userid = 1,
			created_at =  new DateTime(2025, 1, 1, 12, 0, 0),
			updated_at = new DateTime(2025, 1, 1, 12, 0, 0),
			slug = "Admin-User",
		}
	];
}