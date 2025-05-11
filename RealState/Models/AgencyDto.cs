
using RaelState.Assistant;
using System.ComponentModel.DataAnnotations;

namespace RealState.Models;

public class AgencyDto : BaseDto
{
	[Display(Name = "نام کامل")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public string full_name { get; set; }
	[Display(Name = "موبایل")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public string mobile { get; set; }
	[Display(Name = "شماره ثابت")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public string phone { get; set; }
	[Display(Name = "نام آژانس املاک")]
	public string? agency_name { get; set; }

	[Display(Name = "نام کامل شهر و استان")]
	public string city_province_full_name { get; set; }
	[Display(Name = "شهر")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public long city_id { get; set; }
	public CityDto? city { get; set; }

	public UserDto? user { get; set; }
}
