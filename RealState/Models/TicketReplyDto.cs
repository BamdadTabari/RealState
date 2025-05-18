using DataLayer;
using RaelState.Assistant;
using System.ComponentModel.DataAnnotations;

namespace RealState.Models;

public class TicketReplyDto : BaseDto
{
	[Display(Name = "پیام")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public string message { get; set; }

	[Display(Name = "عکس")]
	public string? picture { get; set; }
	[Display(Name = "عکس")]
	public IFormFile? picture_file{ get; set; }

	[Display(Name = "تیکت")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public long ticket_id { get; set; }

	//public TicketDto? Ticket { get; set; }
	public bool is_admin { get; set; }

	[Display(Name = "کاربر")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public long user_id { get; set; }

	//public UserDto? user { get; set; }
}
