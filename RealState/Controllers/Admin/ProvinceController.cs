using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Mvc;
using RaelState.Models;
using RealState.Models;

namespace RealState.Controllers.Admin;
[Route("api/province")]
[ApiController]
public class ProvinceController(IUnitOfWork unitOfWork) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	[HttpGet]
	[Route("list")]
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
		var data = _unitOfWork.ProvinceRepository.GetPaginated(filter);
		return Ok(data);
	}

	[HttpGet]
	[Route("all")]
	public async Task<IActionResult> GetAll()
	{
		var data = await _unitOfWork.ProvinceRepository.GetAll();
		if (data.Count() == 0)
			return Ok(new List<ProvinceDto>());
		return Ok(data.Select(x => new ProvinceDto()
		{
			id = x.id,
			created_at = x.created_at,
			updated_at = x.updated_at,
			slug = x.slug,
			name = x.name,
			cities = x.cities == null ? new List<CityDto>() :
			x.cities.Select(y => new CityDto()
			{
				id = y.id,
				created_at = y.created_at,
				updated_at = y.updated_at,
				slug = y.slug,
				name = y.name,
				province_id = y.province_id,
			}).ToList()
		}).ToList());
	}

	[HttpGet]
	[Route("read/{slug}")]
	public async Task<IActionResult> Detail([FromRoute] string slug)
	{
		var entity = await _unitOfWork.ProvinceRepository.Get(slug);
		if (entity == null)
			return NotFound();
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


	[HttpGet]
	[Route("get/{id}")]
	public async Task<IActionResult> Get([FromRoute] long id)
	{
		var entity = await _unitOfWork.ProvinceRepository.Get(id);
		if (entity == null)
			return NotFound();
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
	[Route("delete/{id}")]
	public async Task<IActionResult> Delete([FromRoute] long id)
	{
		var entity = await _unitOfWork.ProvinceRepository.Get(id);
		if (entity == null)
			return NotFound();
		_unitOfWork.ProvinceRepository.Remove(entity);
		await _unitOfWork.CommitAsync();
		return NoContent();
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
			return BadRequest(error);
		}
		if (await _unitOfWork.ProvinceRepository.ExistsAsync(x => x.name == src.name))
		{
			var error = "مقدار نام شهر تکراریست لطفا تغییر دهید.";
			return BadRequest(error);
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.name);
		if (await _unitOfWork.ProvinceRepository.ExistsAsync(x => x.slug == slug))
		{
			var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(error);
		}

		await _unitOfWork.ProvinceRepository.AddAsync(new Province()
		{
			created_at = DateTime.UtcNow,
			updated_at = DateTime.UtcNow,
			slug = slug,
			name = src.name,
		});
		await _unitOfWork.CommitAsync();
		return Created();
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
			return BadRequest(error);
		}
		var entity = await _unitOfWork.ProvinceRepository.Get(src.id);
		if (entity == null)
			return NotFound();

		if (await _unitOfWork.ProvinceRepository.ExistsAsync(x => x.name == src.name && entity.name != src.name))
		{
			var error = "مقدار نام شهر تکراریست لطفا تغییر دهید.";
			return BadRequest(error);
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.name);
		if (await _unitOfWork.ProvinceRepository.ExistsAsync(x => x.slug == slug && entity.slug != slug))
		{
			var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(error);
		}

		entity.slug = slug;
		entity.updated_at = DateTime.UtcNow;
		entity.name = src.name;
		
		_unitOfWork.ProvinceRepository.Update(entity);
		await _unitOfWork.CommitAsync();
		return NoContent();
	}
}
