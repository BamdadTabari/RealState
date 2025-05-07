using System.ComponentModel.DataAnnotations;
using RaelState.Assistant;

namespace RealState.Models;

public class ContactDto : BaseDto
{
    [Display(Name = "نام کامل")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public string full_name { get; set; }
    [Display(Name = "ایمیل")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public string email { get; set; }
    [Display(Name = "شماره تماس")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public string phone { get; set; }
    [Display(Name = "متن پیام")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public string message { get; set; }
}
