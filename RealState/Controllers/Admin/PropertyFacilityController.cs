using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaelState.Models;
using RealState.Models;

namespace RealState.Controllers.Admin;
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin,MainAdmin")]
public class PropertyFacilityController(IUnitOfWork unitOfWork) : ControllerBase
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
		var data = _unitOfWork.PropertyFacilityRepository.GetPaginated(filter);
		return Ok(new ResponseDto<PaginatedList<PropertyFacility>>()
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
		var data = await _unitOfWork.PropertyFacilityRepository.GetAll();
		if (data.Count() == 0)
			return Ok(new ResponseDto<List<PropertyFacilityDto>>()
			{
				data = new List<PropertyFacilityDto>(),
				is_success = true,
				message = "مقدار ویژگی ملک در دیتابیس وجود ندارد.",
				response_code = 200
			});
		return Ok(new ResponseDto<List<PropertyFacilityDto>>()
		{
			data = data.Select(entity => new PropertyFacilityDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				name = entity.name,
				property_facility_properties = entity.property_facility_properties == null ? 
				new List<PropertyFacilityPropertyDto>() :
				entity.property_facility_properties.Select(y => new PropertyFacilityPropertyDto()
				{
					id = y.id,
					created_at=y.created_at,
					updated_at=y.updated_at,
					property_facility_id = y.property_facility_id,
					property_id = y.property_id,
					slug = y.slug,
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
		var entity = await _unitOfWork.PropertyFacilityRepository.Get(slug);
		if (entity == null)
			return NotFound(new ResponseDto<PropertyFacilityDto>()
			{
				data = new PropertyFacilityDto(),
				is_success = false,
				message = "ویژگی ملک با این slug پیدا نشد",
				response_code = 404
			});
		return Ok(new ResponseDto<PropertyFacilityDto>()
		{
			data = new PropertyFacilityDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				name = entity.name,
				property_facility_properties = entity.property_facility_properties == null ?
				new List<PropertyFacilityPropertyDto>() :
				entity.property_facility_properties.Select(y => new PropertyFacilityPropertyDto()
				{
					id = y.id,
					created_at = y.created_at,
					updated_at = y.updated_at,
					property_facility_id = y.property_facility_id,
					property_id = y.property_id,
					slug = y.slug,
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
		var entity = await _unitOfWork.PropertyFacilityRepository.Get(id);
		if (entity == null)
			return NotFound(new ResponseDto<PropertyFacilityDto>()
			{
				data = new PropertyFacilityDto(),
				is_success = false,
				message = "ویژگی ملک با این ایدی پیدا نشد",
				response_code = 404
			});
		return Ok(new ResponseDto<PropertyFacilityDto>()
		{
			data = new PropertyFacilityDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				name = entity.name,
				property_facility_properties = entity.property_facility_properties == null ?
				new List<PropertyFacilityPropertyDto>() :
				entity.property_facility_properties.Select(y => new PropertyFacilityPropertyDto()
				{
					id = y.id,
					created_at = y.created_at,
					updated_at = y.updated_at,
					property_facility_id = y.property_facility_id,
					property_id = y.property_id,
					slug = y.slug,
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
		var entity = await _unitOfWork.PropertyFacilityRepository.Get(id);
		if (entity == null)
			return NotFound(new ResponseDto<PropertyFacilityDto>()
			{
				data = new PropertyFacilityDto(),
				is_success = false,
				message = "ویژگی ملک با این ایدی پیدا نشد",
				response_code = 404
			});
		_unitOfWork.PropertyFacilityRepository.Remove(entity);
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<PropertyFacilityDto>()
		{
			data = new PropertyFacilityDto(),
			is_success = true,
			message = "ویژگی ملک با موفقیت حذف شد",
			response_code = 204
		});
	}

	[HttpPost]
	[Route("create")]
	public async Task<IActionResult> Create([FromForm] PropertyFacilityDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<BlogDto>()
			{
				data = new BlogDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		if (await _unitOfWork.PropertyFacilityRepository.ExistsAsync(x => x.name == src.name))
		{
			var error = "مقدار نام ویژگی ملک تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<BlogDto>()
			{
				data = new BlogDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.name );
		if (await _unitOfWork.PropertyFacilityRepository.ExistsAsync(x => x.slug == slug))
		{
			var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<BlogDto>()
			{
				data = new BlogDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}

		await _unitOfWork.PropertyFacilityRepository.AddAsync(new PropertyFacility()
		{
			created_at = DateTime.UtcNow,
			updated_at = DateTime.UtcNow,
			slug = slug,
			name = src.name,
		});
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<BlogDto>()
		{
			data = new BlogDto(),
			is_success = true,
			message = "ویژگی ملک با موفقیت ایجاد شد.",
			response_code = 201
		});
	}

	[HttpPost]
	[Route("edit")]
	public async Task<IActionResult> Edit([FromForm] PropertyFacilityDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<BlogDto>()
			{
				data = new BlogDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var entity = await _unitOfWork.PropertyFacilityRepository.Get(src.id);
		if (entity == null)
			return NotFound(new ResponseDto<BlogDto>()
			{
				data = new BlogDto(),
				is_success = false,
				message = "ویژگی ملک با این ایدی پیدا نشد",
				response_code = 400
			});

		if (await _unitOfWork.PropertyFacilityRepository.ExistsAsync(x => x.name == src.name &&
		entity.name != src.name))
		{
			var error = "مقدار نام ویژگی ملک تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<BlogDto>()
			{
				data = new BlogDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.name );
		if (await _unitOfWork.PropertyFacilityRepository.ExistsAsync(x => x.slug == slug && entity.slug != slug))
		{
			var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<BlogDto>()
			{
				data = new BlogDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}

		entity.slug = slug;
		entity.updated_at = DateTime.UtcNow;
		entity.name = src.name;

		_unitOfWork.PropertyFacilityRepository.Update(entity);
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<BlogDto>()
		{
			data = new BlogDto(),
			is_success = true,
			message = "ویژگی ملک با موفقیت ویرایش شد.",
			response_code = 204
		});
	}
}