using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaelState.Models;
using RealState.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RealState.Controllers.Admin;
[Route("api/plan")]
[ApiController]
[Authorize(Roles = "Admin,MainAdmin")]
public class PlanController(IUnitOfWork unitOfWork) : ControllerBase
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
		var data = _unitOfWork.PlanRepository.GetPaginated(filter);
		return Ok(new ResponseDto<PaginatedList<Plan>>()
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
		var data = await _unitOfWork.PlanRepository.GetAll();
		if (data.Count() == 0)
			return Ok(new ResponseDto<List<PlanDto>>()
			{
				data = new List<PlanDto>(),
				is_success = true,
				message = "مقدار پلن در دیتابیس وجود ندارد.",
				response_code = 200
			});
		return Ok(new ResponseDto<List<PlanDto>>()
		{
			data = data.Select(entity => new PlanDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				name = entity.name,
				description = entity.description,
				price = entity.price,
				plan_months = entity.plan_months,
				property_count = entity.property_count
			}).ToList(),
			is_success=true,
			message = "",
			response_code =200
		});
	}

	[HttpGet]
	[Route("read/{slug}")]
	public async Task<IActionResult> Detail([FromRoute] string slug)
	{
		var entity = await _unitOfWork.PlanRepository.Get(slug);
		if (entity == null)
			return NotFound(new ResponseDto<PlanDto>()
			{
				data = new PlanDto(),
				is_success = false,
				message = "پلن با این slug پیدا نشد",
				response_code = 404
			});
		return Ok(new ResponseDto<PlanDto>()
		{
			data = new PlanDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				name = entity.name,
				description = entity.description,
				price = entity.price,
				plan_months = entity.plan_months,
				property_count = entity.property_count
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
		var entity = await _unitOfWork.PlanRepository.Get(id);
		if (entity == null)
			return NotFound(new ResponseDto<PlanDto>()
			{
				data = new PlanDto(),
				is_success = false,
				message = "پلن با این ایدی پیدا نشد",
				response_code = 404
			});
		return Ok(new ResponseDto<PlanDto>()
		{
			data = new PlanDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				name = entity.name,
				description = entity.description,
				price = entity.price,
				plan_months = entity.plan_months,
				property_count = entity.property_count
			},
			is_success=true,
			message = "",
			response_code = 200
		});
	}

	[HttpPost]
	[Route("delete")]
	public async Task<IActionResult> Delete([FromBody] long id)
	{
		var entity = await _unitOfWork.PlanRepository.Get(id);
		if (entity == null)
			return NotFound(new ResponseDto<PlanDto>()
			{
				data = new PlanDto(),
				is_success = false,
				message = "پلن با این ایدی پیدا نشد",
				response_code = 404
			});
		_unitOfWork.PlanRepository.Remove(entity);
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<PlanDto>()
		{
			data = new PlanDto(),
			is_success = true,
			message = "پلن با موفقیت حذف شد",
			response_code = 204
		});
	}

	[HttpPost]
	[Route("create")]
	public async Task<IActionResult> Create([FromForm] PlanDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<PlanDto>()
			{
				data = new PlanDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		if (await _unitOfWork.PlanRepository.ExistsAsync(x => x.name == src.name))
		{
			var error = "مقدار نام تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<PlanDto>()
			{
				data = new PlanDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.name);
		if (await _unitOfWork.PlanRepository.ExistsAsync(x => x.slug == slug))
		{
			var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<PlanDto>()
			{
				data = new PlanDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}

		await _unitOfWork.PlanRepository.AddAsync(new Plan()
		{
			created_at = DateTime.UtcNow,
			updated_at = DateTime.UtcNow,
			slug = slug,
			name = src.name,
			property_count = src.property_count,
			plan_months = src.plan_months,
			price = src.price,
			description = src.description
		});
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<PlanDto>()
		{
			data = new PlanDto(),
			is_success = true,
			message = "پلن با موفقیت ایجاد شد.",
			response_code = 201
		});
	}

	[HttpPost]
	[Route("edit")]
	public async Task<IActionResult> Edit([FromForm] PlanDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<PlanDto>()
			{
				data = new PlanDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var entity = await _unitOfWork.PlanRepository.Get(src.id);
		if (entity == null)
			return NotFound(new ResponseDto<PlanDto>()
			{
				data = new PlanDto(),
				is_success = false,
				message = "پلن با این ایدی پیدا نشد",
				response_code = 404
			});

		if (await _unitOfWork.PlanRepository.ExistsAsync(x => x.name == src.name && entity.name != src.name))
		{
			var error = "مقدار نام تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<PlanDto>()
			{
				data = new PlanDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.name);
		if (await _unitOfWork.PlanRepository.ExistsAsync(x => x.slug == slug && entity.slug != slug))
		{
			var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<PlanDto>()
			{
				data = new PlanDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}

		entity.slug = slug;
		entity.updated_at = DateTime.UtcNow;
		entity.name = src.name;
		entity.description = src.description;
		entity.price = src.price;
		entity.plan_months = src.plan_months;
		entity.property_count = src.property_count;

		_unitOfWork.PlanRepository.Update(entity);
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<PlanDto>()
		{
			data = new PlanDto(),
			is_success = true,
			message = "پلن با موفقیت ویرایش شد.",
			response_code = 204
		});
	}
}
