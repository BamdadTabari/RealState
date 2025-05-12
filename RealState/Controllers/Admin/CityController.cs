using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaelState.Models;
using RealState.Models;
using RealState.Requests;
using RealState.RequestsAndQueries;

namespace RaelState.Controllers.Admin;
[Route("api/city")]
[ApiController]
[Authorize(Roles = "Admin,MainAdmin")]
public class CityController(IUnitOfWork unitOfWork) : ControllerBase
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
		var data = _unitOfWork.CityRepository.GetPaginated(filter);
		return Ok(new ResponseDto<PaginatedList<City>>()
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
		var data = await _unitOfWork.CityRepository.GetAll();
		if (data.Count() == 0)
			return Ok(new ResponseDto<List<CityDto>>()
			{
				data = null,
				is_success = true,
				message = "مقدار شهر در دیتابیس وجود ندارد.",
				response_code = 200
			});
		return Ok(new ResponseDto<List<CityDto>>()
		{
			data = data.Select(entity => new CityDto()
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
			}).ToList(),
			is_success = true,
			message = "",
			response_code = 200
		});
	}

	[HttpGet]
	[Route("read/{slug}")]
	public async Task<IActionResult> Detail(GetQuery<string> src )
	{
		var entity = await _unitOfWork.CityRepository.Get(src.data);
		if (entity == null)
			return NotFound(new ResponseDto<CityDto>()
			{
				data = null,
				is_success = false,
				message = "شهر با این slug پیدا نشد",
				response_code = 404
			});
		return Ok(new ResponseDto<CityDto>()
		{
			data = new CityDto()
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
			},
			is_success = true,
			message = "",
			response_code = 200,
		});
	}


	[HttpGet]
	[Route("get/{id}")]
	public async Task<IActionResult> Get(GetQuery<long> src)
	{
		var entity = await _unitOfWork.CityRepository.Get(src.data);
		if (entity == null)
			return NotFound(new ResponseDto<CityDto>()
			{
				data = null,
				is_success = false,
				message = "شهر با این ایدی پیدا نشد",
				response_code = 404
			});
		return Ok(new ResponseDto<CityDto>()
		{
			data = new CityDto()
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
			},
			is_success = true,
			message = "",
			response_code = 200
		});
	}

	[HttpPost]
	[Route("delete")]
	public async Task<IActionResult> Delete(DeleteRequest<long> src)
	{
		var entity = await _unitOfWork.CityRepository.Get(src.data);
		if (entity == null)
			return NotFound(new ResponseDto<CityDto>()
			{
				data = null,
				is_success = false,
				message = "شهر با این ایدی پیدا نشد",
				response_code = 404
			});
		_unitOfWork.CityRepository.Remove(entity);
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<CityDto>()
		{
			data = null,
			is_success = true,
			message = "شهر با موفقیت حذف شد",
			response_code = 204
		});
	}

	[HttpPost]
	[Route("create")]
	public async Task<IActionResult> Create(CreateRequest<CityDto> src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<CityDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		if (await _unitOfWork.CityRepository.ExistsAsync(x => x.name == src.data.name && x.province_id == src.data.province_id))
		{
			var error = "مقدار نام شهر تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<CityDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var slug = src.data.slug ?? SlugHelper.GenerateSlug(src.data.name + src.data.province_id);
		if (await _unitOfWork.CityRepository.ExistsAsync(x => x.slug == slug))
		{
			var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<CityDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}

		await _unitOfWork.CityRepository.AddAsync(new City()
		{
			created_at = DateTime.UtcNow,
			updated_at = DateTime.UtcNow,
			slug = slug,
			name = src.data.name,
			province_id = src.data.province_id,
		});
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<CityDto>()
		{
			data = null,
			is_success = true,
			message = "شهر با موفقیت ایجاد شد.",
			response_code = 201
		});
	}

	[HttpPost]
	[Route("edit")]
	public async Task<IActionResult> Edit(EditRequest<CityDto> src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<CityDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var entity = await _unitOfWork.CityRepository.Get(src.data.id);
		if (entity == null)
			return NotFound(new ResponseDto<CityDto>()
			{
				data = null,
				is_success = false,
				message = "شهر با این ایدی پیدا نشد",
				response_code = 400
			});

		if (await _unitOfWork.CityRepository.ExistsAsync(x => x.name == src.data.name && entity.name != src.data.name
		&& x.province_id == src.data.province_id))
		{
			var error = "مقدار نام شهر تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<CityDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var slug = src.data.slug ?? SlugHelper.GenerateSlug(src.data.name + src.data.province_id);
		if (await _unitOfWork.CityRepository.ExistsAsync(x => x.slug == slug && entity.slug != slug))
		{
			var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<CityDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}

		entity.slug = slug;
		entity.updated_at = DateTime.UtcNow;
		entity.name = src.data.name;
		entity.province_id = src.data.province_id;

		_unitOfWork.CityRepository.Update(entity);
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<CityDto>()
		{
			data = null,
			is_success = true,
			message = "شهر با موفقیت ویرایش شد.",
			response_code = 204
		});
	}
}
