using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Mvc;
using RealState.Models;

namespace RealState.Controllers.Admin;
[Route("api/plan")]
[ApiController]
public class PlanController(IUnitOfWork unitOfWork) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	[HttpGet]
	[Route("home")]
	public async Task<IActionResult> Index(string? searchTerm,
		SortByEnum sortBy = SortByEnum.CreationDate,
		int page = 1,
		int pageSize = 10)
	{
		// Set up filter object
		var filter = new DefaultPaginationFilter(page, pageSize)
		{
			Keyword = searchTerm,
			SortBy = sortBy,
		};
		var data = _unitOfWork.PlanRepository.GetPaginated(filter);
		return Ok(data);
	}

	[HttpGet]
	[Route("get-all-plans")]
	public async Task<IActionResult> GetAll()
	{
		var data = await _unitOfWork.PlanRepository.GetAll();
		if (data.Count() == 0)
			return Ok(new List<PlanDto>());
		return Ok(data.Select(x => new PlanDto()
		{
			id = x.id,
			created_at = x.created_at,
			updated_at = x.updated_at,
			slug = x.slug,
			name = x.name,
			description = x.description,
			price = x.price,
			plan_months = x.plan_months,
			property_count = x.property_count
		}).ToList());
	}

	[HttpGet]
	[Route("city-detail/{slug}")]
	public async Task<IActionResult> Detail([FromRoute] string slug)
	{
		var entity = await _unitOfWork.PlanRepository.Get(slug);
		if (entity == null)
			return NotFound();
		return Ok(new PlanDto()
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
		});
	}


	[HttpGet]
	[Route("get-city/{id}")]
	public async Task<IActionResult> Get([FromRoute] long id)
	{
		var entity = await _unitOfWork.PlanRepository.Get(id);
		if (entity == null)
			return NotFound();
		return Ok(new PlanDto()
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
		});
	}

	[HttpPost]
	[Route("city-delete/{id}")]
	public async Task<IActionResult> Delete([FromRoute] long id)
	{
		var entity = await _unitOfWork.PlanRepository.Get(id);
		if (entity == null)
			return NotFound();
		_unitOfWork.PlanRepository.Remove(entity);
		await _unitOfWork.CommitAsync();
		return NoContent();
	}

	[HttpPost]
	[Route("create-city")]
	public async Task<IActionResult> Create([FromForm] PlanDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(error);
		}
		if (await _unitOfWork.PlanRepository.ExistsAsync(x => x.name == src.name))
		{
			var error = "مقدار نام تکراریست لطفا تغییر دهید.";
			return BadRequest(error);
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.name);
		if (await _unitOfWork.PlanRepository.ExistsAsync(x => x.slug == slug))
		{
			var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(error);
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
		return NoContent();
	}

	[HttpPost]
	[Route("edit-city")]
	public async Task<IActionResult> Edit([FromForm] PlanDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(error);
		}
		var entity = await _unitOfWork.PlanRepository.Get(src.id);
		if (entity == null)
			return NotFound();

		if (await _unitOfWork.PlanRepository.ExistsAsync(x => x.name == src.name && entity.name != src.name))
		{
			var error = "مقدار نام تکراریست لطفا تغییر دهید.";
			return BadRequest(error);
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.name);
		if (await _unitOfWork.PlanRepository.ExistsAsync(x => x.slug == slug && entity.slug != slug))
		{
			var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(error);
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
		return NoContent();
	}
}
