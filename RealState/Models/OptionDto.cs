using System.ComponentModel.DataAnnotations;
using RaelState.Assistant;

namespace RaelState.Models;

public class OptionDto : BaseDto
{
    [Display(Name = "کلید آپشن")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public string option_key { get; set; }
    [Display(Name = "مقدار آپشن")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public string option_value { get; set; }
}
