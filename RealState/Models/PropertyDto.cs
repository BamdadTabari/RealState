using DataLayer;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RaelState.Assistant;
using System.ComponentModel.DataAnnotations;

namespace RealState.Models;

public class PropertyDto : BaseDto
{
	[Display(Name = "نام")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public string name { get; set; }
	[Display(Name = "توضیحات")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public string description { get; set; }
	[Display(Name = "کد")]
	public string? code { get; set; }

	
	//public List<string> gallery { get; set; }

	//public List<IFormFile> gallery_files { get; set; }

	[Display(Name = "وضعیت ملک")]
	public AdStatusEnum state_enum { get; set; } = AdStatusEnum.NotConfirmed;
	[Display(Name = "آدرس")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public string address { get; set; }
	[Display(Name = "نوع فروش")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public TypeEnum type_enum { get; set; }
	[Display(Name = "نام کامل شهر و استان")]
	public string city_province_full_name { get; set; }
	[Display(Name = "شهر")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public long city_id { get; set; }
	public City? city { get; set; }
	[Display(Name = "متراژ")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public decimal meterage { get; set; }
	[Display(Name = "دسته بندی")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public long category_id { get; set; }
	[ValidateNever]
	public PropertyCategoryDto? property_category { get; set; }
	[Display(Name = "برای فروش است؟")]
	public bool is_for_sale { get; set; }
	[Display(Name = "قیمت فروش")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public decimal sell_price { get; set; }
	/// <summary>
	/// rahn
	/// </summary>
	[Display(Name = "پیش پرداخت")]
	public decimal? mortgage_price { get; set; }
	/// <summary>
	/// ejare
	/// </summary>
	[Display(Name = "اجاره")]
	public decimal? rent_price { get; set; }

	[Display(Name = "تعداد اتاق خواب")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public int bed_room_count { get; set; }

	[Display(Name = "سن بنا")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public int property_age { get; set; }
	[Display(Name = "طبقه")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public int property_floor { get; set; }
	[ValidateNever]
	public ICollection<PropertyFacilityPropertyDto>? property_facility_properties { get; set; }
	[Display(Name = "وضعیت")]
	[Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
	public long situation_id { get; set; }
	//[ValidateNever]
	//public PropertySituationDto? situation { get; set; }
	public long? owner_id { get; set; }
	//[ValidateNever]
	//public User? user { get; set; }

	[Display(Name = "ویدیو")]
	public string? video { get; set; }
	[Display(Name = "ویدیو")]
	public IFormFile? video_file { get; set; }
	[Display(Name = "متن ویدیو")]
	public string? video_caption { get; set; }

	[Display(Name = "گالری")]
	[ValidateNever]
	public ICollection<PropertyGalleryDto>? gallery { get; set; }

}

