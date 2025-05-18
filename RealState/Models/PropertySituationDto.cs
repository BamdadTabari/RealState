using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RaelState.Assistant;
using System.ComponentModel.DataAnnotations;

namespace RealState.Models;

public class PropertySituationDto : BaseDto
{
	[Display(Name = "نام")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public string name { get; set; }
	[ValidateNever]
	public ICollection<PropertyDto>? properties { get; set; }
}
