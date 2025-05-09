using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Mvc;
using RaelState.Assistant;
using RaelState.Models;
using RealState.Models;

namespace RaelState.Controllers.Admin;
[Route("api/city")]
[ApiController]
public class CityController(IUnitOfWork unitOfWork) : ControllerBase
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
		var data = _unitOfWork.CityRepository.GetPaginated(filter);
		return Ok(data);
	}

	[HttpGet]
	[Route("get-all-cities")]
	public async Task<IActionResult> GetAll()
	{
		var data = await _unitOfWork.CityRepository.GetAll();
		if (data.Count() == 0)
			return Ok(new List<CityDto>());
		return Ok(data.Select(x => new CityDto()
		{
			id = x.id,
			created_at = x.created_at,
			updated_at = x.updated_at,
			slug = x.slug,
			name = x.name,
			province_id = x.province_id,
			province = new ProvinceDto()
			{
				name = x.province.name,
				id = x.province.id,
				created_at = x.province.created_at,
				updated_at = x.province.updated_at,
				slug = x.province.slug,
			},
			agency_list = x.agency_list == null ? new List<AgencyDto>():
			x.agency_list.Select(y => new AgencyDto()
			{
				id = y.id,
				created_at = y.created_at,
				updated_at = y.updated_at,
				slug = y.slug,
				agency_name = y.agency_name,
				city_id = y.city_id,
				city_province_full_name = y.city_province_full_name,
				full_name = y.full_name,
				mobile = y.mobile,
				phone = y.phone,
			}).ToList()
		}).ToList());
	}

	[HttpGet]
	[Route("city-detail/{slug}")]
	public async Task<IActionResult> Detail([FromRoute] string slug)
	{
		var entity = await _unitOfWork.CityRepository.Get(slug);
		if (entity == null)
			return NotFound();
		return Ok(new CityDto()
		{
			id = entity.id,
			created_at = entity.created_at,
			updated_at = entity.updated_at,
			slug = entity.slug,
			name = entity.name,
			province_id = entity.province_id,
			province = new ProvinceDto()
			{
				name = entity.province.name,
				id = entity.province.id,
				created_at = entity.province.created_at,
				updated_at = entity.province.updated_at,
				slug = entity.province.slug,
			},
			agency_list = entity.agency_list == null ? new List<AgencyDto>() :
			entity.agency_list.Select(y => new AgencyDto()
			{
				id = y.id,
				created_at = y.created_at,
				updated_at = y.updated_at,
				slug = y.slug,
				agency_name = y.agency_name,
				city_id = y.city_id,
				city_province_full_name = y.city_province_full_name,
				full_name = y.full_name,
				mobile = y.mobile,
				phone = y.phone,
			}).ToList()
		});
	}


	[HttpGet]
	[Route("get-city/{id}")]
	public async Task<IActionResult> Get([FromRoute] long id)
	{
		var entity = await _unitOfWork.CityRepository.Get(id);
		if (entity == null)
			return NotFound();
		return Ok(new CityDto()
		{
			id = entity.id,
			created_at = entity.created_at,
			updated_at = entity.updated_at,
			slug = entity.slug,
			name = entity.name,
			province_id = entity.province_id,
			province = new ProvinceDto()
			{
				name = entity.province.name,
				id = entity.province.id,
				created_at = entity.province.created_at,
				updated_at = entity.province.updated_at,
				slug = entity.province.slug,
			},
			agency_list = entity.agency_list == null ? new List<AgencyDto>() :
			entity.agency_list.Select(y => new AgencyDto()
			{
				id = y.id,
				created_at = y.created_at,
				updated_at = y.updated_at,
				slug = y.slug,
				agency_name = y.agency_name,
				city_id = y.city_id,
				city_province_full_name = y.city_province_full_name,
				full_name = y.full_name,
				mobile = y.mobile,
				phone = y.phone,
			}).ToList()
		});
	}

	[HttpPost]
	[Route("city-delete/{id}")]
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
	public async Task<IActionResult> Create([FromForm] CityDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(error);
		}
		if (await _unitOfWork.CityRepository.ExistsAsync(x => x.name == src.name && x.province_id == src.province_id))
		{
			var error = "مقدار نام شهر تکراریست لطفا تغییر دهید.";
			return BadRequest(error);
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.name + src.province_id);
		if (await _unitOfWork.CityRepository.ExistsAsync(x => x.slug == slug))
		{
			var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(error);
		}

		await _unitOfWork.CityRepository.AddAsync(new City()
		{
			created_at = DateTime.UtcNow,
			updated_at = DateTime.UtcNow,
			slug = slug,
			name = src.name,
			province_id = src.province_id,
		});
		await _unitOfWork.CommitAsync();
		return Ok();
	}

	[HttpPost]
	[Route("edit-city")]
	public async Task<IActionResult> Edit([FromForm] CityDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(error);
		}
		var entity = await _unitOfWork.CityRepository.Get(src.id);
		if (entity == null)
			return NotFound();

		if (await _unitOfWork.CityRepository.ExistsAsync(x => x.name == src.name && entity.name != src.name
		&& x.province_id == src.province_id))
		{
			var error = "مقدار نام شهر تکراریست لطفا تغییر دهید.";
			return BadRequest(error);
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.name + src.province_id);
		if (await _unitOfWork.CityRepository.ExistsAsync(x => x.slug == slug && entity.slug != slug))
		{
			var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(error);
		}

		entity.slug = slug;
		entity.updated_at = DateTime.UtcNow;
		entity.name = src.name;
		entity.province_id = src.province_id;

		_unitOfWork.CityRepository.Update(entity);
		await _unitOfWork.CommitAsync();
		return Ok();
	}
}
