using DataLayer;
using RaelState.Assistant;
using System.ComponentModel.DataAnnotations;

namespace RealState.Models;

public class ContactDto : BaseDto
{
    [Display(Name = "متن پیام")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public string message { get; set; }
	[Display(Name = "کاربر")]
	public long user_id { get; set; }
	public User user { get; set; }
	[Display(Name = "آیا ادمین است؟")]
	public bool is_admin { get; set; }
}
