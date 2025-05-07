using System.ComponentModel.DataAnnotations;

namespace RaelState.Models;

public class TokenRequestDto
{
    [Display(Name = "Access Token")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public string token { get; set; } // Access Token قبلی (منقضی شده)
    [Display(Name = "Refresh Token")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public string refresh_token { get; set; }
}

