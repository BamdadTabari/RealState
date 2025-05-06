namespace DataLayer;
public static class OptionSeed
{
    public static List<Option> All =>
    [
    new()
        {
            Id = 1,
            OptionKey = "AdminMobile",
            OptionValue = "09301724389",
            CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0),
            UpdatedAt = new DateTime(2025, 1, 1, 12, 0, 0),
            Slug = "AdminMobile"
        },
         new()
        {
            Id = 2,
            OptionKey = "Telegram",
            OptionValue = "https://t.me/your_username",
            CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0),
            UpdatedAt = new DateTime(2025, 1, 1, 12, 0, 0),
            Slug = "Telegram"
        },
         new()
        {
            Id = 3,
            OptionKey = "Whatsapp",
            OptionValue = "https://wa.me/989123456789",
            CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0),
            UpdatedAt = new DateTime(2025, 1, 1, 12, 0, 0),
            Slug = "Whatsapp"
        },
         new()
        {
            Id = 4,
            OptionKey = "Instagram",
            OptionValue = "https://www.instagram.com/your_username",
            CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0),
            UpdatedAt = new DateTime(2025, 1, 1, 12, 0, 0),
            Slug = "Instagram"
        },
         new()
        {
            Id = 5,
            OptionKey = "SendSmsToUser",
            OptionValue = "true",
            CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0),
            UpdatedAt = new DateTime(2025, 1, 1, 12, 0, 0),
            Slug = "SendSmsToUser"
        }
         ,
         new()
        {
            Id = 6,
            OptionKey = "SendEmailToUser",
            OptionValue = "true",
            CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0),
            UpdatedAt = new DateTime(2025, 1, 1, 12, 0, 0),
            Slug = "SendEmailToUser"

        },
         new()
        {
            Id = 7,
            OptionKey = "ReserveTimeOutInMinute",
            OptionValue = "10",
            CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0),
            UpdatedAt = new DateTime(2025, 1, 1, 12, 0, 0),
            Slug = "ReserveTimeOutInMinute"
        }
    ];
}
