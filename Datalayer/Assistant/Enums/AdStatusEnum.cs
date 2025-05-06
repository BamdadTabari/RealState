using System.ComponentModel.DataAnnotations;

namespace DataLayer;
public enum AdStatusEnum
{
	[Display(Name = "تایید شده")]
	Confirmed = 0,

	[Display(Name = "در انتظار تایید")]
	NotConfirmed = 1,

	[Display(Name = "آرشیو")]
	Archived = 2,

	[Display(Name = "رد شد")]
	Cancel = 3,
}
