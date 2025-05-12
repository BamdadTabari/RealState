using System.ComponentModel.DataAnnotations;
using RaelState.Assistant;

namespace RaelState.Models;

public class AdminUserForRegistrationCommand : BaseDto
{
	[Display(Name = "نام کاربری")]
	[Required(ErrorMessage = "لطفا مقدار {0} را وارد کنید")]
	public string user_name { get; set; }

	[Display(Name = "ایمیل")]
	[Required(ErrorMessage = "لطفا مقدار {0} را وارد کنید")]
	public string email { get; set; }
	[Display(Name = "شماره تماس")]
	[Required(ErrorMessage = "لطفا مقدار {0} را وارد کنید")]
	public string mobile { get; set; }

	[Display(Name = "پسورد")]
	[Required(ErrorMessage = "لطفا مقدار {0} را وارد کنید")]
	public string password { get; set; }

	public bool is_active { get; set; }
}
