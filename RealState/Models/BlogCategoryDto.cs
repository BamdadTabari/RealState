using System.ComponentModel.DataAnnotations;
using RaelState.Assistant;
using RaelState.Models;

namespace RaelState;

public class BlogCategoryDto : BaseDto
{
    [Display(Name = "نام دسته بندی")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public string name { get; set; }
    [Display(Name = "توضیحات دسته بندی")]
    public string? description { get; set; }

    [Display(Name = "مقالات")]
    public List<BlogDto>? blogs { get; set; }
}
