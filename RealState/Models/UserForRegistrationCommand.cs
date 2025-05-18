using RaelState.Assistant;
using System.ComponentModel.DataAnnotations;

namespace RealState.Models;

public class UserForRegistrationCommand 
{
	[Display(Name = "نام کاربری")]
	[Required(ErrorMessage = "لطفا مقدار {0} را وارد کنید")]
	public string user_name { get; set; }

	[Display(Name = "ایمیل")]
	[Required(ErrorMessage = "لطفا مقدار {0} را وارد کنید")]
	public string email { get; set; }
	[Display(Name = "شماره تماس")]
	[Required(ErrorMessage = "لطفا مقدار {0} را وارد کنید")]
	public string phone_number { get; set; }

	public AgencyDto Agency { get; set; }

	[Display(Name = "آیا آژانس املاک است؟")]
	public bool is_agency { get; set; }
}
