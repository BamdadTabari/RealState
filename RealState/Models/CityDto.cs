using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RaelState.Assistant;
using RaelState.Models;
using System.ComponentModel.DataAnnotations;

namespace RealState.Models;

public class CityDto : BaseDto
{
    [Display(Name = "عنوان")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public string name { get; set; }

    [Display(Name = "استان")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public long province_id { get; set; }
	[ValidateNever]
	public ProvinceDto? province { get; set; }
	[ValidateNever]
	public ICollection<AgencyDto>? agency_list { get; set; }
}
