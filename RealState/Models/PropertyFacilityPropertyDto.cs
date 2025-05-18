using DataLayer;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RaelState.Assistant;
using System.ComponentModel.DataAnnotations;

namespace RealState.Models;

public class PropertyFacilityPropertyDto : BaseDto
{
	[Display(Name = "ملک")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public long property_id { get; set; }
	//[ValidateNever]
	//public Property? property { get; set; }

	[Display(Name = "امکانات ملک")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public long property_facility_id { get; set; }
	[ValidateNever]
	public PropertyFacilityDto? property_facility { get; set; }
}
