using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaelState.Models;
using RealState.Models;

namespace RealState.Controllers.Admin;
[Route("api/property-situation")]
[ApiController]
[Authorize(Roles = "Admin,MainAdmin")]
public class PropertySituationController(IUnitOfWork unitOfWork) : ControllerBase
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
		var data = _unitOfWork.PropertySituationRepository.GetPaginated(filter);
		return Ok(new ResponseDto<PaginatedList<PropertySituation>>()
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
		var data = await _unitOfWork.PropertySituationRepository.GetAll();
		if (data.Count() == 0)
			return Ok(new ResponseDto<List<PropertySituationDto>>()
			{
				data = null,
				is_success = true,
				message = "مقدار وضعیت ملک در دیتابیس وجود ندارد.",
				response_code = 200
			});
		return Ok(new ResponseDto<List<PropertySituationDto>>()
		{
			data = data.Select(entity => new PropertySituationDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				name = entity.name,
				properties = entity.properties == null ? new List<PropertyDto>() :
				entity.properties.Select(y => new PropertyDto()
				{
					id = y.id,
					created_at = y.created_at,
					updated_at = y.updated_at,
					slug = y.slug,
					name = y.name,
					address = y.address,
					bed_room_count = y.bed_room_count,
					category_id = y.category_id,
					city_id = y.city_id,
					city_province_full_name = y.city_province_full_name,
					code = y.code,
					description = y.description,
					expire_date = y.expire_date,
					gallery = y.gallery,
					is_for_sale = y.is_for_sale,
					meterage = y.meterage,
					mortgage_price = y.meterage,
					property_age = y.property_age,
					property_floor = y.property_floor,
					rent_price = y.rent_price,
					sell_price = y.sell_price,
					situation_id = y.situation_id,
					state_enum = y.state_enum,
					type_enum = y.type_enum,
				}).ToList()
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
		var entity = await _unitOfWork.PropertySituationRepository.Get(slug);
		if (entity == null)
			return NotFound(new ResponseDto<PropertySituationDto>()
			{
				data = null,
				is_success = false,
				message = "وضعیت ملک با این slug پیدا نشد",
				response_code = 404
			});
		return Ok(new ResponseDto<PropertySituationDto>()
		{
			data = new PropertySituationDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				name = entity.name,
				properties = entity.properties == null ? new List<PropertyDto>() :
				entity.properties.Select(y => new PropertyDto()
				{
					id = y.id,
					created_at = y.created_at,
					updated_at = y.updated_at,
					slug = y.slug,
					name = y.name,
					address = y.address,
					bed_room_count = y.bed_room_count,
					category_id = y.category_id,
					city_id = y.city_id,
					city_province_full_name = y.city_province_full_name,
					code = y.code,
					description = y.description,
					expire_date = y.expire_date,
					gallery = y.gallery,
					is_for_sale = y.is_for_sale,
					meterage = y.meterage,
					mortgage_price = y.meterage,
					property_age = y.property_age,
					property_floor = y.property_floor,
					rent_price = y.rent_price,
					sell_price = y.sell_price,
					situation_id = y.situation_id,
					state_enum = y.state_enum,
					type_enum = y.type_enum,
				}).ToList()
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
		var entity = await _unitOfWork.PropertySituationRepository.Get(id);
		if (entity == null)
			return NotFound(new ResponseDto<PropertySituationDto>()
			{
				data = null,
				is_success = false,
				message = "وضعیت ملک با این ایدی پیدا نشد",
				response_code = 404
			});
		return Ok(new ResponseDto<PropertySituationDto>()
		{
			data = new PropertySituationDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				name = entity.name,
				properties = entity.properties == null ? new List<PropertyDto>() :
				entity.properties.Select(y => new PropertyDto()
				{
					id = y.id,
					created_at = y.created_at,
					updated_at = y.updated_at,
					slug = y.slug,
					name = y.name,
					address = y.address,
					bed_room_count = y.bed_room_count,
					category_id = y.category_id,
					city_id = y.city_id,
					city_province_full_name = y.city_province_full_name,
					code = y.code,
					description = y.description,
					expire_date = y.expire_date,
					gallery = y.gallery,
					is_for_sale = y.is_for_sale,
					meterage = y.meterage,
					mortgage_price = y.meterage,
					property_age = y.property_age,
					property_floor = y.property_floor,
					rent_price = y.rent_price,
					sell_price = y.sell_price,
					situation_id = y.situation_id,
					state_enum = y.state_enum,
					type_enum = y.type_enum,
				}).ToList()
			},
			is_success = true,
			message = "",
			response_code = 200
		});
	}

	[HttpPost]
	[Route("delete")]
	public async Task<IActionResult> Delete([FromForm] long id)
	{
		var entity = await _unitOfWork.PropertySituationRepository.Get(id);
		if (entity == null)
			return NotFound(new ResponseDto<PropertySituationDto>()
			{
				data = null,
				is_success = false,
				message = "وضعیت ملک با این ایدی پیدا نشد",
				response_code = 404
			});
		_unitOfWork.PropertySituationRepository.Remove(entity);
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<PropertySituationDto>()
		{
			data = null,
			is_success = true,
			message = "وضعیت ملک با موفقیت حذف شد",
			response_code = 204
		});
	}

	[HttpPost]
	[Route("create")]
	public async Task<IActionResult> Create([FromForm] PropertySituationDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<PropertySituationDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		if (await _unitOfWork.PropertySituationRepository.ExistsAsync(x => x.name == src.name))
		{
			var error = "مقدار نام وضعیت ملک تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<PropertySituationDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.name);
		if (await _unitOfWork.PropertySituationRepository.ExistsAsync(x => x.slug == slug))
		{
			var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<PropertySituationDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}

		await _unitOfWork.PropertySituationRepository.AddAsync(new PropertySituation()
		{
			created_at = DateTime.UtcNow,
			updated_at = DateTime.UtcNow,
			slug = slug,
			name = src.name,
		});
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<PropertySituationDto>()
		{
			data = null,
			is_success = true,
			message = "وضعیت ملک با موفقیت ایجاد شد.",
			response_code = 201
		});
	}

	[HttpPost]
	[Route("edit")]
	public async Task<IActionResult> Edit([FromForm] PropertySituationDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<PropertySituationDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var entity = await _unitOfWork.PropertySituationRepository.Get(src.id);
		if (entity == null)
			return NotFound(new ResponseDto<PropertySituationDto>()
			{
				data = null,
				is_success = false,
				message = "وضعیت ملک با این ایدی پیدا نشد",
				response_code = 400
			});

		if (await _unitOfWork.PropertySituationRepository.ExistsAsync(x => x.name == src.name &&
		entity.name != src.name))
		{
			var error = "مقدار نام وضعیت ملک تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<PropertySituationDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.name);
		if (await _unitOfWork.PropertySituationRepository.ExistsAsync(x => x.slug == slug && entity.slug != slug))
		{
			var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<PropertySituationDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}

		entity.slug = slug;
		entity.updated_at = DateTime.UtcNow;
		entity.name = src.name;

		_unitOfWork.PropertySituationRepository.Update(entity);
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<PropertySituationDto>()
		{
			data = null,
			is_success = true,
			message = "وضعیت ملک با موفقیت ویرایش شد.",
			response_code = 204
		});
	}
}