using System.ComponentModel.DataAnnotations;

namespace RealState.Models;

public class EditAgencyCommand
{
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
	[Display(Name = "آیدی")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public long agency_id { get; set; }

}
