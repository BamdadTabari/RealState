using System.ComponentModel.DataAnnotations;

namespace DataLayer;
public enum TypeEnum
{
	[Display(Name = " رهن و اجاره")]
	Rental = 0,
	[Display(Name = "فروش")]
	Sell = 1,
}
