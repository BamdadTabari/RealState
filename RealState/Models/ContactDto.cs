using RaelState.Assistant;
using System.ComponentModel.DataAnnotations;

namespace RealState.Models;

public class ContactDto : BaseDto
{
	[Display(Name = "نام کامل")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public string full_name { get; set; }
	[Display(Name = "موبایل")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public string mobile { get; set; }
	[Display(Name = "پیام")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public string message { get; set; }
}
