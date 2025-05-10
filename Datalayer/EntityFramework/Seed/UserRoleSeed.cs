namespace DataLayer;

public static class UserRoleSeed
{
	public static List<UserRole> All =>
	[
		new()
		{
			role_id = 3,
			user_id = 1,
			created_at =  new DateTime(2025, 1, 1, 12, 0, 0),
			updated_at = new DateTime(2025, 1, 1, 12, 0, 0),
			slug = "Main-Admin-User",
		}
	];
}