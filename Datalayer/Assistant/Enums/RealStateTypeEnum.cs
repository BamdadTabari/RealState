using System.ComponentModel.DataAnnotations;

namespace DataLayer;
public enum RealStateTypeEnum
{
	[Display(Name = "رهن")]
	Mortgage = 0,
	[Display(Name = "اجاره")]
	Rental = 1,
	[Display(Name = "فروش")]
	Sell = 2,
}
