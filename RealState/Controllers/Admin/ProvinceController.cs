using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaelState.Models;
using RealState.Models;

namespace RealState.Controllers.Admin;
[Route("api/province")]
[ApiController]
[Authorize(Roles = "Admin,MainAdmin")]
public class ProvinceController(IUnitOfWork unitOfWork) : ControllerBase
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
		var data = _unitOfWork.ProvinceRepository.GetPaginated(filter);
		return Ok(new ResponseDto<PaginatedList<Province>>()
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
		var data = await _unitOfWork.ProvinceRepository.GetAll();
		if (data.Count() == 0)
			return Ok(new ResponseDto<List<ProvinceDto>>()
			{
				data = null,
				is_success = true,
				message = "مقدار استان در دیتابیس وجود ندارد.",
				response_code = 200
			});
		return Ok(new ResponseDto<List<ProvinceDto>>()
		{
			data = data.Select(entity => new ProvinceDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				name = entity.name,
				cities = entity.cities == null ? new List<CityDto>() :
			entity.cities.Select(y => new CityDto()
			{
				id = y.id,
				created_at = y.created_at,
				updated_at = y.updated_at,
				slug = y.slug,
				name = y.name,
				province_id = y.province_id,
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
		var entity = await _unitOfWork.ProvinceRepository.Get(slug);
		if (entity == null)
			return NotFound(new ResponseDto<ProvinceDto>()
			{
				data = null,
				is_success = false,
				message = "استان با این slug پیدا نشد",
				response_code = 404
			});
		return Ok(new ResponseDto<ProvinceDto>()
		{
			data = new ProvinceDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				name = entity.name,
				cities = entity.cities == null ? new List<CityDto>() :
			entity.cities.Select(y => new CityDto()
			{
				id = y.id,
				created_at = y.created_at,
				updated_at = y.updated_at,
				slug = y.slug,
				name = y.name,
				province_id = y.province_id,
			}).ToList()
			},
			is_success = true,
			message = "",
			response_code = 200
		});
	}


	[HttpGet]
	[Route("get/{id}")]
	public async Task<IActionResult> Get([FromRoute] long id)
	{
		var entity = await _unitOfWork.ProvinceRepository.Get(id);
		if (entity == null)
			return NotFound(new ResponseDto<ProvinceDto>()
			{
				data = null,
				is_success = false,
				message = "استان با این ایدی پیدا نشد",
				response_code = 404
			});
		return Ok(new ProvinceDto()
		{
			id = entity.id,
			created_at = entity.created_at,
			updated_at = entity.updated_at,
			slug = entity.slug,
			name = entity.name,
			cities = entity.cities == null ? new List<CityDto>() :
			entity.cities.Select(y => new CityDto()
			{
				id = y.id,
				created_at = y.created_at,
				updated_at = y.updated_at,
				slug = y.slug,
				name = y.name,
				province_id = y.province_id,
			}).ToList()
		});
	}

	[HttpPost]
	[Route("delete")]
	public async Task<IActionResult> Delete([FromForm] long id)
	{
		var entity = await _unitOfWork.ProvinceRepository.Get(id);
		if (entity == null)
			return NotFound(new ResponseDto<ProvinceDto>()
			{
				data = null,
				is_success = false,
				message = "استان با این ایدی پیدا نشد",
				response_code = 404
			});
		_unitOfWork.ProvinceRepository.Remove(entity);
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<ProvinceDto>()
		{
			data = null,
			is_success = true,
			message = "استان با موفقیت حذف شد",
			response_code = 204
		});
	}

	[HttpPost]
	[Route("create")]
	public async Task<IActionResult> Create([FromForm] ProvinceDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<ProvinceDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		if (await _unitOfWork.ProvinceRepository.ExistsAsync(x => x.name == src.name))
		{
			var error = "مقدار نام تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<ProvinceDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.name);
		if (await _unitOfWork.ProvinceRepository.ExistsAsync(x => x.slug == slug))
		{
			var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<ProvinceDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}

		await _unitOfWork.ProvinceRepository.AddAsync(new Province()
		{
			created_at = DateTime.UtcNow,
			updated_at = DateTime.UtcNow,
			slug = slug,
			name = src.name,
		});
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<ProvinceDto>()
		{
			data = null,
			is_success = true,
			message = "استان با موفقیت ایجاد شد.",
			response_code = 201
		});
	}

	[HttpPost]
	[Route("edit")]
	public async Task<IActionResult> Edit([FromForm] ProvinceDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<ProvinceDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var entity = await _unitOfWork.ProvinceRepository.Get(src.id);
		if (entity == null)
			return NotFound(new ResponseDto<ProvinceDto>()
			{
				data = null,
				is_success = false,
				message = "استان با این ایدی پیدا نشد",
				response_code = 404
			});

		if (await _unitOfWork.ProvinceRepository.ExistsAsync(x => x.name == src.name && entity.name != src.name))
		{
			var error = "مقدار نام تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<ProvinceDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.name);
		if (await _unitOfWork.ProvinceRepository.ExistsAsync(x => x.slug == slug && entity.slug != slug))
		{
			var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<ProvinceDto>()
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
		
		_unitOfWork.ProvinceRepository.Update(entity);
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<ProvinceDto>()
		{
			data = null,
			is_success = true,
			message = "استان با موفقیت ویرایش شد.",
			response_code = 204
		});
	}
}
