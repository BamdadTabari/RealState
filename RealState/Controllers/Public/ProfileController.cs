using Azure.Core;
using DataLayer;
using Microsoft.AspNetCore.Mvc;
using RaelState;
using RealState.Models;

namespace RealState.Controllers.Public;
[Route("api/public/profile")]
[ApiController]
public class ProfileController(JwtTokenService tokenService, IUnitOfWork unitOfWork) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	public static readonly SecurityTokenConfig Config = new();
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
	[Route("me")]
	public async Task<IActionResult> Profile()
	{
		var userId = GetCurrentUserId();
		var user = await _unitOfWork.UserRepository.GetUser(userId);
		if (user == null) 
			return NotFound(new ResponseDto<UserDto>()
			{
				data = new UserDto(),
				is_success = false,
				message = "کاربر پیدا نشد. وارد شوید یا ثبت نام کنید",
				response_code = 404
			});

		return Ok(new ResponseDto<UserDto>()
		{
			data = new UserDto()
			{
				id = user.id,
				created_at = user.created_at,
				updated_at = user.updated_at,
				expire_date = user.expire_date,
				slug = user.slug,
				email = user.email,
				is_active = user.is_active,
				is_agency = user.is_agency,
				is_licensed = user.is_licensed,
				is_locked_out = user.is_locked_out,
				failed_login_count = user.failed_login_count,
				last_login_date_time = user.last_login_date_time,
				is_mobile_confirmed = user.is_mobile_confirmed,
				license = user.license,
				mobile = user.mobile,
				property_count = user.property_count,
				user_name = user.user_name,
				agency = new AgencyDto()
				{
					id = user.agency.id,
					created_at = user.agency.created_at,
					updated_at = user.agency.updated_at,
					city_id = user.agency.city_id,
					agency_name = user.agency.agency_name,
					city_province_full_name = user.agency.city_province_full_name,
					full_name = user.agency.full_name,
					mobile = user.agency.mobile,
					phone = user.agency.phone,
					slug = user.agency.slug,
				}
			},
		});
	}

	[HttpGet]
	[Route("edit-agency")]
	public async Task<IActionResult> EditAgency([FromForm] EditAgencyCommand src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<BlogCategoryDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var agency = await _unitOfWork.AgencyRepository.Get(src.agency_id);
		if (agency == null)
			return NotFound(new ResponseDto<EditAgencyCommand>()
			{
				data = null,
				is_success = false,
				message = "آژانس یافت نشد",
				response_code = 404
			});

		var city = await _unitOfWork.CityRepository.Get(src.city_id);
		if (city == null)
			return NotFound(new ResponseDto<CityDto>()
			{
				data = null,
				is_success = false,
				message = "شهر پیدا نشد",
				response_code = 404
			});
		var userId = GetCurrentUserId();
		var user = await _unitOfWork.UserRepository.GetUser(userId);
		if (user == null)
			return NotFound(new ResponseDto<CityDto>()
			{
				data = null,
				is_success = false,
				message = "کاربر پیدا نشد",
				response_code = 404
			});
		var agencySlug = SlugHelper.GenerateSlug(src.full_name + Guid.NewGuid().ToString());
		if (await _unitOfWork.AgencyRepository.ExistsAsync(x => x.slug == agencySlug))
		{
			var error = "آژانس املاک با این نامک وجود دارد";
			return BadRequest(new ResponseDto<AgencyDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		
		agency.updated_at = DateTime.Now;
		agency.slug = agencySlug;
		agency.mobile = src.agency_mobile;
		agency.phone = src.agency_phone;
		agency.agency_name = src.agency_name;
		agency.city_id = src.city_id;
		agency.city_province_full_name = city.name + $"({city.province.name})";
		agency.full_name = src.full_name;
		agency.user_id = user.id;
		
		_unitOfWork.AgencyRepository.Update(agency);
		await _unitOfWork.CommitAsync();

		return Ok(new ResponseDto<string>()
		{
			data = null,
			message = "عملیات موفقیت آمیز بود",
			is_success=true,
			response_code = 204
		});
	}


	[HttpGet]
	[Route("edit-user")]
	public async Task<IActionResult> EditUser([FromForm] EditAgencyCommand src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<BlogCategoryDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var agency = await _unitOfWork.AgencyRepository.Get(src.agency_id);
		if (agency == null)
			return NotFound(new ResponseDto<EditAgencyCommand>()
			{
				data = null,
				is_success = false,
				message = "آژانس یافت نشد",
				response_code = 404
			});

		var city = await _unitOfWork.CityRepository.Get(src.city_id);
		if (city == null)
			return NotFound(new ResponseDto<CityDto>()
			{
				data = null,
				is_success = false,
				message = "شهر پیدا نشد",
				response_code = 404
			});
		var userId = GetCurrentUserId();
		var user = await _unitOfWork.UserRepository.GetUser(userId);
		if (user == null)
			return NotFound(new ResponseDto<CityDto>()
			{
				data = null,
				is_success = false,
				message = "کاربر پیدا نشد",
				response_code = 404
			});
		var agencySlug = SlugHelper.GenerateSlug(src.full_name + Guid.NewGuid().ToString());
		if (await _unitOfWork.AgencyRepository.ExistsAsync(x => x.slug == agencySlug))
		{
			var error = "آژانس املاک با این نامک وجود دارد";
			return BadRequest(new ResponseDto<AgencyDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}

		agency.updated_at = DateTime.Now;
		agency.slug = agencySlug;
		agency.mobile = src.agency_mobile;
		agency.phone = src.agency_phone;
		agency.agency_name = src.agency_name;
		agency.city_id = src.city_id;
		agency.city_province_full_name = city.name + $"({city.province.name})";
		agency.full_name = src.full_name;
		agency.user_id = user.id;

		_unitOfWork.AgencyRepository.Update(agency);
		await _unitOfWork.CommitAsync();

		return Ok(new ResponseDto<string>()
		{
			data = null,
			message = "عملیات موفقیت آمیز بود",
			is_success = true,
			response_code = 204
		});
	}
}
