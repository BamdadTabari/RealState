using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RaelState.Assistant;
using System.ComponentModel.DataAnnotations;

namespace RaelState.Models;

public class BlogDto : BaseDto
{
    [Display(Name = "نام")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public string name { get; set; }
    [Display(Name = "توضیخات کوتاه")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public string description { get; set; }
    [Display(Name = "فایل تصویر شاخص")]
    public IFormFile? image_file { get; set; }
    [Display(Name = "آدرس تصویر شاخص")]
    public string? image { get; set; }
    [Display(Name = "ALT تصویر شاخص")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public string image_alt { get; set; }
    [Display(Name = "متن مقاله")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public string blog_text { get; set; }
    [Display(Name = "نمایش داده شود")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public bool show_blog { get; set; }
    [Display(Name = "کلمات کلیدی")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public string keyWords { get; set; }


    [Display(Name = "آیدی دسته بندی مقاله")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public long blog_category_id { get; set; }

    [Display(Name = "دسته بندی مقاله")]
	[ValidateNever]
	public BlogCategoryDto? blog_category { get; set; }
}
