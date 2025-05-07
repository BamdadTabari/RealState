using RaelState.Assistant;
using RealState.Models;
using System.ComponentModel.DataAnnotations;

namespace RaelState.Models;

public class RoleDto : BaseDto
{
    [Display(Name = "نام")]
    [Required(ErrorMessage = "لطفا مقدار {0}را وارد کنید.")]
    public string title { get; set; }

    #region Navigations

    public ICollection<UserRoleDto>? user_roles { get; set; }

    #endregion
}
