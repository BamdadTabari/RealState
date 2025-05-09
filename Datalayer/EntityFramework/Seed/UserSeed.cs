
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
			password_hash = "omTtMfA5EEJCzjH5t/Q67cRXK5TRwerSqN7sJSm41No=.FRLmTm9jwMcEFnjpjgivJw==", // QAZqaz!@#123
			concurrency_stamp = "X3JO2EOCURAEBU6HHY6OBYEDD2877FXU",
			security_stamp = "098NTB7E5LFFXREHBSEHDKLI0DOBIKST",
			created_at = new DateTime(2025, 1, 1, 12, 0, 0),
			updated_at = new DateTime(2025, 1, 1, 12, 0, 0),
			slug= "Admin-User",
			is_active = true
		}
	];
}