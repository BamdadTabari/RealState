using System.ComponentModel.DataAnnotations;

namespace TicketApi.Models;

public class VerifyPhoneDto
{
	[Display(Name = "کد تایید")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public int confirm_code { get; set; }
	[Display(Name = "شماره تماس")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public string phone_number { get; set; }
}
