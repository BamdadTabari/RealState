using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RealState;

public class AdminLoginDto
{
    [Display(Name = "نام کاربری یا ایمیل")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public string user_name_or_email { get; set; }
    [Display(Name = "پسورد")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public string password { get; set; }
}
