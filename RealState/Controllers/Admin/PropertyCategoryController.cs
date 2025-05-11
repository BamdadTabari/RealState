using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaelState.Models;
using RealState.Models;

namespace RealState.Controllers.Admin;
[Route("api/property-category")]
[ApiController]
[Authorize(Roles = "Admin,MainAdmin")]
public class PropertyCategoryController(IUnitOfWork unitOfWork) : ControllerBase
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
		var data = _unitOfWork.PropertyCategoryRepository.GetPaginated(filter);
		return Ok(new ResponseDto<PaginatedList<PropertyCategory>>()
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
		var data = await _unitOfWork.PropertyCategoryRepository.GetAll();
		if (data.Count() == 0)
			return Ok(new ResponseDto<List<PropertyCategoryDto>>()
			{
				data = new List<PropertyCategoryDto>(),
				is_success = true,
				message = "مقدار دسته بندی ملک در دیتابیس وجود ندارد.",
				response_code = 200
			});
		return Ok(new ResponseDto<List<PropertyCategoryDto>>()
		{
			data = data.Select(entity => new PropertyCategoryDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				name = entity.name,
				properties = entity.properties == null ? new List<PropertyDto>():
				entity.properties.Select(p => new PropertyDto()
				{
					id = p.id,
					created_at = p.created_at,
					updated_at= p.updated_at,
					slug = p.slug,
					name = p.name,
					address = p.address,
					bed_room_count = p.bed_room_count,
					category_id	= p.category_id,
					city_id = p.city_id,
					city_province_full_name = p.city_province_full_name,
					code = p.code,
					description = p.description,
					expire_date = p.expire_date,
					gallery = p.gallery,
					is_for_sale = p.is_for_sale,
					meterage = p.meterage,
					mortgage_price = p.mortgage_price,
					property_age = p.property_age,
					property_floor = p.property_floor,
					rent_price = p.rent_price,
					sell_price= p.sell_price,
					situation_id = p.situation_id,
					state_enum = p.state_enum,
					type_enum = p.type_enum,
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
		var entity = await _unitOfWork.PropertyCategoryRepository.Get(slug);
		if (entity == null)
			return NotFound(new ResponseDto<PropertyCategoryDto>()
			{
				data = new PropertyCategoryDto(),
				is_success = false,
				message = "دسته بندی ملک با این slug پیدا نشد",
				response_code = 404
			});
		return Ok(new ResponseDto<PropertyCategoryDto>()
		{
			data = new PropertyCategoryDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				name = entity.name,
				properties = entity.properties == null ? new List<PropertyDto>() :
				entity.properties.Select(p => new PropertyDto()
				{
					id = p.id,
					created_at = p.created_at,
					updated_at = p.updated_at,
					slug = p.slug,
					name = p.name,
					address = p.address,
					bed_room_count = p.bed_room_count,
					category_id = p.category_id,
					city_id = p.city_id,
					city_province_full_name = p.city_province_full_name,
					code = p.code,
					description = p.description,
					expire_date = p.expire_date,
					gallery = p.gallery,
					is_for_sale = p.is_for_sale,
					meterage = p.meterage,
					mortgage_price = p.mortgage_price,
					property_age = p.property_age,
					property_floor = p.property_floor,
					rent_price = p.rent_price,
					sell_price = p.sell_price,
					situation_id = p.situation_id,
					state_enum = p.state_enum,
					type_enum = p.type_enum,
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
		var entity = await _unitOfWork.PropertyCategoryRepository.Get(id);
		if (entity == null)
			return NotFound(new ResponseDto<PropertyCategoryDto>()
			{
				data = new PropertyCategoryDto(),
				is_success = false,
				message = "دسته بندی ملک با این ایدی پیدا نشد",
				response_code = 404
			});
		return Ok(new ResponseDto<PropertyCategoryDto>()
		{
			data = new PropertyCategoryDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				name = entity.name,
				properties = entity.properties == null ? new List<PropertyDto>() :
				entity.properties.Select(p => new PropertyDto()
				{
					id = p.id,
					created_at = p.created_at,
					updated_at = p.updated_at,
					slug = p.slug,
					name = p.name,
					address = p.address,
					bed_room_count = p.bed_room_count,
					category_id = p.category_id,
					city_id = p.city_id,
					city_province_full_name = p.city_province_full_name,
					code = p.code,
					description = p.description,
					expire_date = p.expire_date,
					gallery = p.gallery,
					is_for_sale = p.is_for_sale,
					meterage = p.meterage,
					mortgage_price = p.mortgage_price,
					property_age = p.property_age,
					property_floor = p.property_floor,
					rent_price = p.rent_price,
					sell_price = p.sell_price,
					situation_id = p.situation_id,
					state_enum = p.state_enum,
					type_enum = p.type_enum,
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
		var entity = await _unitOfWork.PropertyCategoryRepository.Get(id);
		if (entity == null)
			return NotFound(new ResponseDto<PropertyCategoryDto>()
			{
				data = new PropertyCategoryDto(),
				is_success = false,
				message = "دسته بندی ملک با این ایدی پیدا نشد",
				response_code = 404
			});
		_unitOfWork.PropertyCategoryRepository.Remove(entity);
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<PropertyCategoryDto>()
		{
			data = new PropertyCategoryDto(),
			is_success = true,
			message = "دسته بندی ملک با موفقیت حذف شد",
			response_code = 204
		});
	}

	[HttpPost]
	[Route("create")]
	public async Task<IActionResult> Create([FromForm] PropertyCategoryDto src)
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
		if (await _unitOfWork.PropertyCategoryRepository.ExistsAsync(x => x.name == src.name))
		{
			var error = "مقدار نام دسته بندی ملک تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<BlogDto>()
			{
				data = new BlogDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.name);
		if (await _unitOfWork.PropertyCategoryRepository.ExistsAsync(x => x.slug == slug))
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

		await _unitOfWork.PropertyCategoryRepository.AddAsync(new PropertyCategory()
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
			message = "دسته بندی ملک با موفقیت ایجاد شد.",
			response_code = 201
		});
	}

	[HttpPost]
	[Route("edit")]
	public async Task<IActionResult> Edit([FromForm] PropertyCategoryDto src)
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
		var entity = await _unitOfWork.PropertyCategoryRepository.Get(src.id);
		if (entity == null)
			return NotFound(new ResponseDto<BlogDto>()
			{
				data = new BlogDto(),
				is_success = false,
				message = "دسته بندی ملک با این ایدی پیدا نشد",
				response_code = 400
			});

		if (await _unitOfWork.PropertyCategoryRepository.ExistsAsync(x => x.name == src.name 
		&& entity.name != src.name))
		{
			var error = "مقدار نام دسته بندی ملک تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<BlogDto>()
			{
				data = new BlogDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.name);
		if (await _unitOfWork.PropertyCategoryRepository.ExistsAsync(x => x.slug == slug && entity.slug != slug))
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

		_unitOfWork.PropertyCategoryRepository.Update(entity);
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<BlogDto>()
		{
			data = new BlogDto(),
			is_success = true,
			message = "دسته بندی ملک با موفقیت ویرایش شد.",
			response_code = 204
		});
	}
}