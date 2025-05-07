using RaelState.Assistant;
using System.ComponentModel.DataAnnotations;

namespace RealState.Models;

public class PropertyCategoryDto : BaseDto
{
	[Display(Name = "نام")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public string name { get; set; }

	public ICollection<PropertyDto> properties { get; set; }
}
