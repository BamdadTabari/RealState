﻿using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealState.Models;

namespace RealState.Controllers.Public;
[Route("api/public/dashboard")]
[ApiController]
[Authorize]
public class PublicDashboardController(IUnitOfWork unitOfWork, JwtTokenService tokenService) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly JwtTokenService _tokenService = tokenService;

	[HttpPost]
	[Route("current-user")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public long GetCurrentUserId()
	{
		var userId = _tokenService.GetUserIdFromClaims(User) ?? "0";
		return long.Parse(userId);
	}
	[HttpGet]
	[Route("data")]
	public async Task<IActionResult> Data()
	{
		var userId =  GetCurrentUserId();
		var user = await _unitOfWork.UserRepository.GetUser(userId);
		if (user == null)
			return NotFound(new ResponseDto<UserDashboard>()
			{
				data = new UserDashboard(),
				is_success = false,
				message = "کاربر یافت نشد",
				response_code = 404
			});
		var allProperties = await _unitOfWork.PropertyRepository.FindList(x => x.owner_id == userId);
		var planId = user.plan_id != null ? (long)user.plan_id: 0;
		if(planId == 0)
		{
			return Ok(new ResponseDto<UserDashboard>()
			{
				data = new UserDashboard()
				{
					ads = allProperties.Count(),
					archived_ads = allProperties.Count(x => x.state_enum == AdStatusEnum.Archived),
					rent_ads = allProperties.Count(x => x.type_enum == TypeEnum.Rental),
					sell_ads = allProperties.Count(x => x.type_enum == TypeEnum.Sell),
					days_until_expire = Math.Round((user.expire_date - DateTime.Now).TotalDays),
					plan_months = 1,
					plan_property_count = 5,
					remain_properties = user.property_count,
					plan_description = "رایگان",
					plan_name = "رایگان"
				},
				is_success= true,
				message = "Ok",
				response_code = 200
			});
		}
		else
		{
			var plan = await _unitOfWork.PlanRepository.Get(planId);
			if (plan == null)
				return NotFound(new ResponseDto<UserDashboard>()
				{
					data = new UserDashboard(),
					is_success = false,
					message = "پلن کاربری یافت نشد",
					response_code = 404
				});

			return Ok(new ResponseDto<UserDashboard>()
			{
				data = new UserDashboard()
				{
					ads = allProperties.Count(),
					archived_ads = allProperties.Count(x => x.state_enum == AdStatusEnum.Archived),
					rent_ads = allProperties.Count(x => x.type_enum == TypeEnum.Rental),
					sell_ads = allProperties.Count(x => x.type_enum == TypeEnum.Sell),
					days_until_expire = Math.Round((user.expire_date - DateTime.Now).TotalDays),
					plan_months = plan.plan_months,
					plan_property_count = plan.property_count,
					remain_properties = user.property_count,
					plan_name = plan.name, 
					plan_description = plan.description
				},
				is_success= true,
				message = "Ok",
				response_code = 200
			});
		}
	}

}
