using DataLayer;
using RaelState.Assistant;
using System.ComponentModel.DataAnnotations;

namespace RealState.Models;

public class PlanDto : BaseDto
{
	[Display(Name = "نام")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public string name { get; set; }
	[Display(Name = "توضیحات")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public string description { get; set; }
	[Display(Name = "قیمت")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public decimal price { get; set; }
	[Display(Name = "تعداد ملک")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public int property_count { get; set; }
	[Display(Name = "تعداد ماه")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public int plan_months { get; set; }

	public Order? order { get; set; }
}
