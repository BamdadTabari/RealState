using System.ComponentModel.DataAnnotations;

namespace RealState.Models;

public class LoginDto
{
    [Display(Name = "شماره تماس")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public string phone_number { get; set; }
}

