using DataLayer;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RaelState.Assistant;
using System.ComponentModel.DataAnnotations;

namespace RealState.Models;

public class TicketDto : BaseDto
{
	public string? ticket_code { get; set; }
	[Display(Name = "موضوع")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public string subject { get; set; } // عنوان تیکت
	[Display(Name = "پیام")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public string message { get; set; } // متن اولیه تیکت
	[Display(Name = "عکس")]
	public string? picture { get; set; }
	[Display(Name = "عکس")]
	public IFormFile? picture_file { get; set; }
	[Display(Name = "وضعیت تیکت")]
	public TicketStatus status { get; set; } = TicketStatus.Open;

	[Display(Name = "کاربر")]
	public long user_id { get; set; } // شناسه کاربر
	public string? user_name { get; set; }
	public bool is_admin { get; set; }
	//public UserDto? user { get; set; } // نویگیشن به کاربر
	[ValidateNever]
	public ICollection<TicketReplyDto>? replies { get; set; }
}
