using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealState.Models;

namespace RealState.Controllers.Public;
[Route("api/public/dashboard")]
[ApiController]
[Authorize]
public class DashboardController(IUnitOfWork unitOfWork, JwtTokenService tokenService) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly JwtTokenService _tokenService = tokenService;

	[HttpPost]
	[Route("current-user")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<User?> GetCurrentUser()
	{
		var userId = _tokenService.GetUserIdFromClaims(User) ?? "0";
		var user = await _unitOfWork.UserRepository.GetUser(long.Parse(userId));
		return user;
	}
	//[HttpGet]
	//[Route("list")]
	//public async Task<IActionResult> UserProperties(string? search_term,
	//	SortByEnum sort_by = SortByEnum.CreationDate,
	//	int page = 1,
	//	int page_size = 10)
	//{
	//	var user = await GetCurrentUser();
	//	if (userId == null)
	//		return NotFound(new ResponseDto<UserDto>()
	//		{
	//			data = null,
	//			is_success = false,
	//			message = "کاربر یافت نشد",
	//			response_code = 404
	//		});

	//	var filter = new DefaultPaginationFilter(page, page_size)
	//	{
	//		Keyword = search_term,
	//		SortBy = sort_by,
	//	};
	//	var data = _unitOfWork.PropertyRepository.GetPaginated(filter, userId);
	//	return Ok(new ResponseDto<PaginatedList<Property>>()
	//	{
	//		data = data,
	//		is_success = true,
	//		message = "",
	//		response_code = 200
	//	});
	//}

}
