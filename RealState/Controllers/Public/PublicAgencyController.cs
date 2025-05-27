using Azure.Core;
using DataLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealState.Models;

namespace RealState.Controllers.Public;
[Route("api/public/agency")]
[ApiController]
[Authorize]
public class PublicAgencyController(IUnitOfWork unitOfWork, JwtTokenService tokenService) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly JwtTokenService _tokenService = tokenService;

	[HttpPost]
	[Route("x")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<long> GetCurrentUserId()
	{
		var userId = _tokenService.GetUserIdFromClaims(User) ?? "0";
		var user = await _unitOfWork.UserRepository.GetUser(long.Parse(userId));
		return user.id;
	}

	[HttpPost]
	[Route("y")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<User> GetCurrentUser()
	{
		var userId = _tokenService.GetUserIdFromClaims(User) ?? "0";
		var user = await _unitOfWork.UserRepository.GetUser(long.Parse(userId));
		return user;
	}

	[HttpGet]
	[Route("mine")]
	public async Task<IActionResult> UserAgency()
	{
		var user = await GetCurrentUser();
		var agency = await _unitOfWork.AgencyRepository.GetByUserId(user.id);
		if (agency == null)
			return NotFound(new ResponseDto<AgencyDto>()
			{
				data = null,
				is_success = false,
				message = "آژانس املاک با این ایدی وجود ندارد"
			});
		var city = await _unitOfWork.CityRepository.Get(agency.city_id);
		return Ok(new AgencyDto()
		{
			id = agency.id,
			agency_name = agency.agency_name,
			city_id = agency.city_id,
			city_province_full_name = agency.city_province_full_name,
			created_at = agency.created_at,
			updated_at = agency.updated_at,
			slug = agency.slug,
			full_name = agency.full_name,
			mobile = agency.mobile,
			phone = agency.phone,
			city = new CityDto()
			{
				id = agency.city.id,
				created_at = agency.city.created_at,
				updated_at = agency.city.updated_at,
				slug = agency.city.slug,
				name = agency.city.name,
				province_id = agency.city.province_id
			},
			user = new UserDto()
			{
				id = user.id,
				created_at = user.created_at,
				updated_at = user.updated_at,
				slug = user.slug,
				email = user.email,
				is_active = user.is_active,
				mobile = user.mobile,
				user_name = user.user_name,
				expire_date = user.expire_date,
				failed_login_count = user.failed_login_count,
				is_agency = user.is_agency,
				is_licensed = user.is_licensed,
				is_locked_out = user.is_locked_out,
				is_mobile_confirmed = user.is_mobile_confirmed,
				last_login_date_time = user.last_login_date_time,
				property_count = user.property_count,
			}
		});
	}

	[HttpPost]
	[Route("edit")]
	public async Task<IActionResult> EditAgency([FromForm] AgencyDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<AgencyDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var entity = await _unitOfWork.AgencyRepository.Get(src.id);
		if (entity == null)
			return NotFound(new ResponseDto<AgencyDto>()
			{
				data = null,
				is_success = false,
				message = "آژانس املاک با این ایدی پیدا نشد",
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
		var agencySlug = src.slug ?? SlugHelper.GenerateSlug(src.full_name + Guid.NewGuid().ToString());
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

		entity.updated_at = DateTime.Now;
		entity.mobile = src.mobile;
		entity.phone = src.phone;
		entity.agency_name = src.agency_name;
		entity.city_id = src.city_id;
		entity.city_province_full_name = city.name + $"({city.province.name})";
		entity.full_name = src.full_name;
		entity.slug = agencySlug;

		_unitOfWork.AgencyRepository.Update(entity);
		await _unitOfWork.CommitAsync();

		return Ok(new ResponseDto<string>()
		{
			data = null,
			message = "ویرایش آژانس با موفقیت انجام شد",
			is_success = true,
			response_code = 204
		});
	}

}
