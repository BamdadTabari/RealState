using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaelState.Models;
using RealState.Models;
using System.Xml;

namespace RealState.Controllers.Admin;
[Route("api/agency")]
[ApiController]
[Authorize(Roles = "Admin,MainAdmin")]
public class AgencyController(IUnitOfWork unitOfWork) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	[HttpGet]
	[Route("list")]
	public async Task<IActionResult> Index(string? search_term,
		SortByEnum sort_by = SortByEnum.CreationDate,
		int page = 1,
		int page_size = 10)
	{
		// Set up filter object
		var filter = new DefaultPaginationFilter(page, page_size)
		{
			Keyword = search_term,
			SortBy = sort_by,
		};
		var data = _unitOfWork.AgencyRepository.GetPaginated(filter);
		return Ok(new ResponseDto<PaginatedList<Agency>>()
		{
			data = data,
			is_success = true,
			message = "",
			response_code = 200
		});
	}

	[HttpGet]
	[Route("all")]
	public async Task<IActionResult> GetAll()
	{
		var data = await _unitOfWork.AgencyRepository.GetAll();
		if (data.Count() == 0)
			return Ok(new ResponseDto<List<AgencyDto>>()
			{
				data = new List<AgencyDto>(),
				is_success = true,
				message = "مقدار آژانس املاک در دیتابیس وجود ندارد.",
				response_code = 200
			});
		return Ok(new ResponseDto<List<AgencyDto>>()
		{
			data = data.Select(entity => new AgencyDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				agency_name = entity.agency_name,
				city_id = entity.city_id,
				city_province_full_name = entity.city_province_full_name,
				full_name = entity.full_name,
				mobile = entity.mobile,
				phone = entity.phone,
				city = new CityDto()
				{
					id = entity.city.id,
					created_at = entity.city.created_at,
					updated_at = entity.city.updated_at,
					slug = entity.city.slug,
					name = entity.city.name,
					province_id = entity.city.province_id,
				},
				user = new UserDto()
				{
					id = entity.user.id,
					created_at = entity.user.created_at,
					updated_at = entity.user.updated_at,
					slug = entity.user.slug,
					email = entity.user.email,
					is_active = entity.user.is_active,
					mobile = entity.user.mobile,
					user_name = entity.user.user_name,
				}
			}).ToList(),
			is_success = true,
			message = "",
			response_code = 200
		});
	}

	[HttpGet]
	[Route("read/{slug}")]
	public async Task<IActionResult> Detail([FromRoute] string slug)
	{
		var entity = await _unitOfWork.AgencyRepository.Get(slug);
		if (entity == null)
			return NotFound(new ResponseDto<AgencyDto>()
			{
				data = new AgencyDto(),
				is_success = false,
				message = "آژانس املاک با این slug پیدا نشد",
				response_code = 404
			});
		return Ok(new ResponseDto<AgencyDto>()
		{
			data = new AgencyDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				agency_name = entity.agency_name,
				city_id = entity.city_id,
				city_province_full_name = entity.city_province_full_name,
				full_name = entity.full_name,
				mobile = entity.mobile,
				phone = entity.phone,
				city = new CityDto()
				{
					id = entity.city.id,
					created_at = entity.city.created_at,
					updated_at = entity.city.updated_at,
					slug = entity.city.slug,
					name = entity.city.name,
					province_id = entity.city.province_id,
				},
				user = new UserDto()
				{
					id = entity.user.id,
					created_at = entity.user.created_at,
					updated_at = entity.user.updated_at,
					slug = entity.user.slug,
					email = entity.user.email,
					is_active = entity.user.is_active,
					mobile = entity.user.mobile,
					user_name = entity.user.user_name,
				}
			},			
			is_success = true,
			message = "",
			response_code = 200,
		});
	}


	[HttpGet]
	[Route("get/{id}")]
	public async Task<IActionResult> Get([FromRoute] long id)
	{
		var entity = await _unitOfWork.AgencyRepository.Get(id);
		if (entity == null)
			return NotFound(new ResponseDto<AgencyDto>()
			{
				data = new AgencyDto(),
				is_success = false,
				message = "آژانس املاک با این ایدی پیدا نشد",
				response_code = 404
			});
		return Ok(new ResponseDto<AgencyDto>()
		{
			data = new AgencyDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				agency_name = entity.agency_name,
				city_id = entity.city_id,
				city_province_full_name = entity.city_province_full_name,
				full_name = entity.full_name,
				mobile = entity.mobile,
				phone = entity.phone,
				city = new CityDto()
				{
					id = entity.city.id,
					created_at = entity.city.created_at,
					updated_at = entity.city.updated_at,
					slug = entity.city.slug,
					name = entity.city.name,
					province_id = entity.city.province_id,
				},
				user = new UserDto()
				{
					id = entity.user.id,
					created_at = entity.user.created_at,
					updated_at = entity.user.updated_at,
					slug = entity.user.slug,
					email = entity.user.email,
					is_active = entity.user.is_active,
					mobile = entity.user.mobile,
					user_name = entity.user.user_name,
				}
			},
			is_success = true,
			message = "",
			response_code = 200
		});
	}

	[HttpPost]
	[Route("delete")]
	public async Task<IActionResult> Delete([FromQuery] long id)
	{
		var entity = await _unitOfWork.AgencyRepository.Get(id);
		if (entity == null)
			return NotFound(new ResponseDto<AgencyDto>()
			{
				data = new AgencyDto(),
				is_success = false,
				message = "آژانس املاک با این ایدی پیدا نشد",
				response_code = 404
			});
		_unitOfWork.AgencyRepository.Remove(entity);
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<AgencyDto>()
		{
			data = new AgencyDto(),
			is_success = true,
			message = "آژانس املاک با موفقیت حذف شد",
			response_code = 204
		});
	}
}
