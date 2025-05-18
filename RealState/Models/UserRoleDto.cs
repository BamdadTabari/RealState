using RaelState.Assistant;
using RaelState.Models;
using System.ComponentModel.DataAnnotations;

namespace RealState.Models;

public class UserRoleDto : BaseDto
{
    [Display(Name = "نقش")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public long role_id { get; set; }
    [Display(Name = "کاربر")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public long user_id { get; set; }

    //public UserDto? user { get; set; }
    //public RoleDto? role { get; set; }
}
