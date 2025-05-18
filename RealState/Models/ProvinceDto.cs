using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RaelState.Assistant;
using RealState.Models;
using System.ComponentModel.DataAnnotations;

namespace RaelState.Models;

public class ProvinceDto : BaseDto
{
    [Display(Name = "نام")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public string name { get; set; }
	[ValidateNever]
	public ICollection<CityDto>? cities { get; set; }
}
