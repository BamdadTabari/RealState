using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RaelState.Assistant;
using System.ComponentModel.DataAnnotations;

namespace RealState.Models;

public class UserForRegistrationCommand 
{
	[Display(Name = "نام کاربری")]
	[Required(ErrorMessage = "لطفا مقدار {0} را وارد کنید")]
	public string user_name { get; set; }

	//[Display(Name = "ایمیل")]
	//public string email { get; set; }
	[Display(Name = "شماره تماس")]
	[Required(ErrorMessage = "لطفا مقدار {0} را وارد کنید")]
	public string phone_number { get; set; }
	[Display(Name = "نام کامل")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public string full_name { get; set; }
	[Display(Name = "موبایل")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public string agency_mobile { get; set; }
	[Display(Name = "شماره ثابت")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public string agency_phone { get; set; }
	[Display(Name = "نام آژانس املاک")]
	public string? agency_name { get; set; }
	[Display(Name = "شهر")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public long city_id { get; set; }

	[Display(Name = "آیا آژانس املاک است؟")]
	public bool is_agency { get; set; }
}
