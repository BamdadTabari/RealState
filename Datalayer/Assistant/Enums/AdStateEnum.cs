using System.ComponentModel.DataAnnotations;

namespace DataLayer;
public enum AdStateEnum
{
	[Display(Name = "ثبت شده")]
	Confirmed = 0,

	[Display(Name = "ثبت نشده")]
	NotConfirmed = 1,

	[Display(Name = "آرشیو")]
	Archived = 2,

	[Display(Name = "رهن")]
	Mortgage = 3,

	[Display(Name = "اجاره")]
	Rental = 4,
}
