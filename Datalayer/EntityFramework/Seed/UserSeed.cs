
namespace DataLayer;

public static class UserSeed
{
	public static List<User> All =>
	[
		new()
		{
			id = 1,
			email = "info@avatick.com",
			failed_login_count = 0,
			is_locked_out = false,
			mobile = "09309309393",
			user_name = "admin-user",
			password_hash = PasswordHasher.Hash("QAZqaz!@#123"),
			concurrency_stamp = StampGenerator.CreateSecurityStamp(32),
			security_stamp = StampGenerator.CreateSecurityStamp(32),
			created_at = new DateTime(2025, 1, 1, 12, 0, 0),
			updated_at = new DateTime(2025, 1, 1, 12, 0, 0),
			slug= "Admin-User",
			is_active = true
		}
	];
}