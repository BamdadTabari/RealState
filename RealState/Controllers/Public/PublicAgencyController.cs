using DataLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealState.Models;

namespace RealState.Controllers.Public;
[Route("api/public/agency")]
[ApiController]
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
		var user = await _unitOfWork.UserRepository.GetUser(int.Parse(userId));
		return user.id;
	}

	[HttpPost]
	[Route("y")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<User> GetCurrentUser()
	{
		var userId = _tokenService.GetUserIdFromClaims(User) ?? "0";
		var user = await _unitOfWork.UserRepository.GetUser(int.Parse(userId));
		return user;
	}

	[HttpGet]
	[Route("mine")]
	public async Task<IActionResult> UserAgency()
	{
		var agency = await _unitOfWork.AgencyRepository.GetByUserId(await GetCurrentUserId());
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
			city_province_full_name = city.name + $"({city.province.name})",
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
				id = agency.user.id,
				created_at = agency.user.created_at,
				updated_at = agency.user.updated_at,
				slug = agency.user.slug,
				email = agency.user.email,
				is_active = agency.user.is_active,
				mobile = agency.user.mobile,
				user_name = agency.user.user_name,
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

		entity.updated_at = DateTime.UtcNow;
		entity.mobile = src.mobile;
		entity.phone = src.phone;
		entity.agency_name = src.agency_name;
		entity.city_id = src.city_id;
		//entity.city_province_full_name =  

		return Ok();
	}

}
