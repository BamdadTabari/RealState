using System.ComponentModel.DataAnnotations;

namespace RealState.Models;

public class EditUserCommand
{
	[Display(Name = "آیدی کاربر")]
	[Required(ErrorMessage = "لطفا مقدار {0} را وارد کنید")]
	public long id { get; set; }
	[Display(Name = "نام کاربری")]
	[Required(ErrorMessage = "لطفا مقدار {0} را وارد کنید")]
	public string user_name { get; set; }

	[Display(Name = "آیا آژانس املاک است؟")]
	public bool is_agency { get; set; }
}
