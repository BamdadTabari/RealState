namespace DataLayer;
public static class OptionSeed
{
	public static List<Option> All =>
	[
	new()
		{
			id = 1,
			OptionKey = "AdminMobile",
			OptionValue = "09301724389",
			created_at = new DateTime(2025, 1, 1, 12, 0, 0),
			updated_at = new DateTime(2025, 1, 1, 12, 0, 0),
			slug = "AdminMobile"
		},
		 new()
		{
			id = 2,
			OptionKey = "Telegram",
			OptionValue = "https://t.me/your_username",
			created_at = new DateTime(2025, 1, 1, 12, 0, 0),
			updated_at = new DateTime(2025, 1, 1, 12, 0, 0),
			slug = "Telegram"
		},
		 new()
		{
			id = 3,
			OptionKey = "Whatsapp",
			OptionValue = "https://wa.me/989123456789",
			created_at = new DateTime(2025, 1, 1, 12, 0, 0),
			updated_at = new DateTime(2025, 1, 1, 12, 0, 0),
			slug = "Whatsapp"
		},
		 new()
		{
			id = 4,
			OptionKey = "Instagram",
			OptionValue = "https://www.instagram.com/your_username",
			created_at = new DateTime(2025, 1, 1, 12, 0, 0),
			updated_at = new DateTime(2025, 1, 1, 12, 0, 0),
			slug = "Instagram"
		},
		 new()
		{
			id = 5,
			OptionKey = "SendSmsToUser",
			OptionValue = "true",
			created_at = new DateTime(2025, 1, 1, 12, 0, 0),
			updated_at = new DateTime(2025, 1, 1, 12, 0, 0),
			slug = "SendSmsToUser"
		}
		 ,
		 new()
		{
			id = 6,
			OptionKey = "SendEmailToUser",
			OptionValue = "true",
			created_at = new DateTime(2025, 1, 1, 12, 0, 0),
			updated_at = new DateTime(2025, 1, 1, 12, 0, 0),
			slug = "SendEmailToUser"

		},
		 new()
		{
			id = 7,
			OptionKey = "ReserveTimeOutInMinute",
			OptionValue = "10",
			created_at = new DateTime(2025, 1, 1, 12, 0, 0),
			updated_at = new DateTime(2025, 1, 1, 12, 0, 0),
			slug = "ReserveTimeOutInMinute"
		}
	];
}
