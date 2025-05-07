using System.ComponentModel.DataAnnotations;

namespace RaelState.Assistant;

public class BaseDto
{
    [Display(Name = "آیدی")]
    public long id { get; set; }
    [Display(Name = "نامک (اختیاری)")]
    public string? slug { get; set; }
    [Display(Name = "زمان ایجاد")]
    public DateTime created_at { get; set; }
    [Display(Name = "زمان ویرایش")]
    public DateTime updated_at { get; set; }
}
