
namespace DataLayer;

public static class UserSeed
{
	public static List<User> All =>
	[
		new()
		{
			id = 1,
			Email = "info@avatick.com",
			FailedLoginCount = 0,
			IsLockedOut = false,
			Mobile = "09309309393",
			Username = "admin-user",
			PasswordHash = PasswordHasher.Hash("QAZqaz!@#123"),
			ConcurrencyStamp = StampGenerator.CreateSecurityStamp(32),
			SecurityStamp = StampGenerator.CreateSecurityStamp(32),
			created_at = new DateTime(2025, 1, 1, 12, 0, 0),
			updated_at = new DateTime(2025, 1, 1, 12, 0, 0),
			slug= "Admin-User",
			IsActive = true
		}
	];
}