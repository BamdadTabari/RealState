using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RaelState.Assistant;
using System.ComponentModel.DataAnnotations;

namespace RealState.Models;

public class PropertyGalleryDto : BaseDto
{
	[Display(Name = "عکس")]
	public string? picture { get; set; }
	[Display(Name = "عکس")]
	public IFormFile? picture_file { get; set; }
	[Display(Name = "Alt تصویر")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public string alt { get; set; }
	[Display(Name = "ملک")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public long property_id { get; set; }
	//[ValidateNever]
	//public PropertyDto? property { get; set; }
}
