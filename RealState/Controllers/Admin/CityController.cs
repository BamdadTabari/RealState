using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Mvc;
using RaelState.Assistant;
using RaelState.Models;

namespace RaelState.Controllers.Admin;
[Route("api/city")]
[ApiController]
public class CityController(IUnitOfWork unitOfWork) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	[HttpGet]
	[Route("home")]
	[HasPermission("City.Index")]
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
		var data = _unitOfWork.CityRepository.GetPaginated(filter);
		return Ok(data);
	}

	[HttpGet]
	[Route("get-all-cities")]
	[HasPermission("City.GetAll")]
	public async Task<IActionResult> GetAll()
	{
		var data = await _unitOfWork.CityRepository.GetAll();
		if (data.Count() == 0)
			return Ok(new List<CityDto>());
		return Ok(data.Select(x => new CityDto()
		{
			Id = x.Id,
			CreatedAt = x.CreatedAt,
			UpdatedAt = x.UpdatedAt,
			Slug = x.Slug,
			Name = x.Name,
			ProvinceId = x.ProvinceId,
			Province = new ProvinceDto()
			{
				Name = x.Province.Name,
				Id = x.Province.Id,
				CreatedAt = x.Province.CreatedAt,
				UpdatedAt = x.Province.UpdatedAt,
				Slug = x.Province.Slug,
			}
		}).ToList());
	}

	[HttpGet]
	[Route("city-detail/{slug}")]
	[HasPermission("City.Detail")]
	public async Task<IActionResult> Detail([FromRoute] string slug)
	{
		var entity = await _unitOfWork.CityRepository.Get(slug);
		if (entity == null)
			return NotFound();
		return Ok(new CityDto()
		{
			Id = entity.Id,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt,
			Slug = entity.Slug,
			Name = entity.Name,
			ProvinceId = entity.ProvinceId,
			Province = new ProvinceDto()
			{
				Id = entity.Province.Id,
				Name = entity.Province.Name,
				CreatedAt = entity.Province.CreatedAt,
				UpdatedAt = entity.Province.UpdatedAt,
				Slug = entity.Province.Slug
			}
		});
	}


	[HttpGet]
	[Route("get-city/{id}")]
	[HasPermission("City.Get")]
	public async Task<IActionResult> Get([FromRoute] long id)
	{
		var entity = await _unitOfWork.CityRepository.Get(id);
		if (entity == null)
			return NotFound();
		return Ok(new CityDto()
		{
			Id = entity.Id,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt,
			Slug = entity.Slug,
			Name = entity.Name,
			ProvinceId = entity.ProvinceId,
			Province = new ProvinceDto()
			{
				Id = entity.Province.Id,
				Name = entity.Province.Name,
				CreatedAt = entity.Province.CreatedAt,
				UpdatedAt = entity.Province.UpdatedAt,
				Slug = entity.Province.Slug
			}
		});
	}

	[HttpPost]
	[Route("city-delete/{id}")]
	[HasPermission("City.Delete")]
	public async Task<IActionResult> Delete([FromRoute] long id)
	{
		var entity = await _unitOfWork.CityRepository.Get(id);
		if (entity == null)
			return NotFound();
		_unitOfWork.CityRepository.Remove(entity);
		await _unitOfWork.CommitAsync();
		return Ok();
	}

	[HttpPost]
	[Route("create-city")]
	[HasPermission("City.Create")]
	public async Task<IActionResult> Create([FromForm] CityDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(error);
		}
		if (await _unitOfWork.CityRepository.ExistsAsync(x => x.Name == src.Name && x.ProvinceId == src.ProvinceId))
		{
			var error = "مقدار نام شهر تکراریست لطفا تغییر دهید.";
			return BadRequest(error);
		}
		var slug = src.Slug ?? SlugHelper.GenerateSlug(src.Name + src.ProvinceId);
		if (await _unitOfWork.CityRepository.ExistsAsync(x => x.Slug == slug))
		{
			var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(error);
		}

		await _unitOfWork.CityRepository.AddAsync(new City()
		{
			CreatedAt = DateTime.UtcNow,
			UpdatedAt = DateTime.UtcNow,
			Slug = slug,
			Name = src.Name,
			ProvinceId = src.ProvinceId,
		});
		await _unitOfWork.CommitAsync();
		return Ok();
	}

	[HttpPost]
	[Route("edit-city")]
	[HasPermission("City.Edit")]
	public async Task<IActionResult> Edit([FromForm] CityDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(error);
		}
		var entity = await _unitOfWork.CityRepository.Get(src.Id);
		if (entity == null)
			return NotFound();

		if (await _unitOfWork.CityRepository.ExistsAsync(x => x.Name == src.Name && entity.Name != src.Name
		&& x.ProvinceId == src.ProvinceId))
		{
			var error = "مقدار نام شهر تکراریست لطفا تغییر دهید.";
			return BadRequest(error);
		}
		var slug = src.Slug ?? SlugHelper.GenerateSlug(src.Name + src.ProvinceId);
		if (await _unitOfWork.CityRepository.ExistsAsync(x => x.Slug == slug && entity.Slug != slug))
		{
			var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(error);
		}

		entity.Slug = slug;
		entity.UpdatedAt = DateTime.UtcNow;
		entity.Name = src.Name;
		entity.ProvinceId = src.ProvinceId;

		_unitOfWork.CityRepository.Update(entity);
		await _unitOfWork.CommitAsync();
		return Ok();
	}
}
