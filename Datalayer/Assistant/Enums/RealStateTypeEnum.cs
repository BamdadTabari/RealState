using System.ComponentModel.DataAnnotations;

namespace DataLayer;
public enum RealStateTypeEnum
{
	[Display(Name = "رهن")]
	Mortgage = 0,
	[Display(Name = "اجاره")]
	Rent = 1,
	[Display(Name = "فروش")]
	Sell = 2,
}
