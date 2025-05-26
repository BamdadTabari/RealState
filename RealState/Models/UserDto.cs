using DataLayer;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RaelState.Assistant;
using System.ComponentModel.DataAnnotations;

namespace RealState.Models;

public class UserDto : BaseDto
{
    #region Identity
    [Display(Name = "نام کاربری")]
    public string user_name { get; set; }

    [Display(Name = "شماره تماس")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public string mobile { get; set; }
    public bool is_mobile_confirmed { get; set; }
    [Display(Name = "ایمیل")]
    public string email { get; set; }

	[Display(Name = "پسورد")]
	public string? password { get; set; }
	#endregion

	#region Login

	public string? password_hash { get; set; }

    public int? failed_login_count { get; set; }
    public DateTime? lock_out_end_time { get; set; }

    public DateTime? last_login_date_time { get; set; }

    #endregion

    #region Management

    public string? security_stamp { get; set; }
    public string? concurrency_stamp { get; set; }
	public bool? is_locked_out { get; set; }
	public bool is_active { get; set; }
	public string? refresh_token { get; set; }
	public DateTime refresh_token_expiry_time { get; set; }
	public bool is_agency { get; set; }
	public bool is_licensed { get; set; }
	public string? license { get; set; }
	#endregion

	#region Navigations
	[ValidateNever]
	public ICollection<UserRoleDto>? user_roles { get; set; }
	[ValidateNever]
	public AgencyDto? agency { get; set; } = new();

	//public ICollection<PropertyDto>? properties { get; set; }

	public long? plan_id { get; set; }
	//[ValidateNever]
	//public PlanDto? plan { get; set; } = new();

	public DateTime expire_date { get; set; }
	public int property_count { get; set; }
	#endregion
}
