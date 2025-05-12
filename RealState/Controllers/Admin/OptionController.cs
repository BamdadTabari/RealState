using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaelState.Models;
using RealState.Models;
using RealState.RequestsAndQueries;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RaelState.Controllers.Admin;
[Route("api/option")]
[ApiController]
[Authorize(Roles = "Admin,MainAdmin")]
public class OptionController(IUnitOfWork unitOfWork) : ControllerBase
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
        var data = _unitOfWork.OptionRepository.GetPaginated(filter);
		return Ok(new ResponseDto<PaginatedList<Option>>()
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
        var data = await _unitOfWork.OptionRepository.GetAll();
        if (data.Count() == 0)
			return Ok(new ResponseDto<List<OptionDto>>()
			{
				data = null,
				is_success = true,
				message = "مقدار آپشن در دیتابیس وجود ندارد.",
				response_code = 200
			});
		return Ok(new ResponseDto<List<OptionDto>>()
        {
            data = data.Select(entity => new OptionDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				option_key = entity.option_key,
				option_value = entity.option_value,
			}).ToList(),
            is_success = true,
            message = "",
            response_code = 200
		});
    }

    [HttpGet]
    [Route("read/{slug}")]
    public async Task<IActionResult> Detail(GetQuery<string> src)
    {
        var entity = await _unitOfWork.OptionRepository.Get(src.data);
        if (entity == null)
			return NotFound(new ResponseDto<OptionDto>()
			{
				data = null,
				is_success = false,
				message = "آپشن با این slug پیدا نشد.",
				response_code = 404
			});
		return Ok(new ResponseDto<OptionDto>()
        {
            data = new OptionDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				option_key = entity.option_key,
				option_value = entity.option_value,
			},
            is_success=true,
            message = "",
            response_code = 200
		});
    }


    [HttpGet]
    [Route("get/{id}")]
    public async Task<IActionResult> Get([FromRoute] long id)
    {
        var entity = await _unitOfWork.OptionRepository.Get(id);
        if (entity == null)
			return NotFound(new ResponseDto<OptionDto>()
			{
				data = null,
				is_success = false,
				message = "آپشن با این ایدی پیدا نشد",
				response_code = 404
			});
		return Ok(new ResponseDto<OptionDto>()
        {
            data = new OptionDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				option_key = entity.option_key,
				option_value = entity.option_value,
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
        var entity = await _unitOfWork.OptionRepository.Get(id);
        if (entity == null)
			return NotFound(new ResponseDto<OptionDto>()
			{
				data = null,
				is_success = false,
				message = "آپشن با این ایدی پیدا نشد",
				response_code = 404
			});
		_unitOfWork.OptionRepository.Remove(entity);
        await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<OptionDto>()
		{
			data = null,
			is_success = true,
			message = "آپشن با موفقیت حذف شد",
			response_code = 204
		});
	}

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create([FromForm] OptionDto src)
    {
        if (!ModelState.IsValid)
        {
            var error = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<OptionDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
        if (await _unitOfWork.OptionRepository.ExistsAsync(x => x.option_key == src.option_key))
        {
            var error = "مقدار کلید تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<OptionDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
        var slug = src.slug ?? SlugHelper.GenerateSlug(src.option_key);
        if (await _unitOfWork.OptionRepository.ExistsAsync(x => x.slug == slug))
        {
            var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<OptionDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}

        await _unitOfWork.OptionRepository.AddAsync(new Option()
        {
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow,
            slug = slug,
            option_key = src.option_key,
            option_value = src.option_value,
        });
        await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<OptionDto>()
		{
			data = null,
			is_success = true,
			message = "آپشن با موفقیت ایجاد شد.",
			response_code = 201
		});
	}

    [HttpPost]
    [Route("edit")]
    public async Task<IActionResult> Edit([FromForm] OptionDto src)
    {
        if (!ModelState.IsValid)
        {
            var error = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<OptionDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
        var entity = await _unitOfWork.OptionRepository.Get(src.id);
        if (entity == null)
			return NotFound(new ResponseDto<OptionDto>()
			{
				data = null,
				is_success = false,
				message = "آپشن با این ایدی پیدا نشد.",
				response_code = 400
			});

		if (await _unitOfWork.OptionRepository.ExistsAsync(x => x.option_key == src.option_key && entity.option_key != src.option_key))
        {
            var error = "مقدار کلید تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<OptionDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
        var slug = src.slug ?? SlugHelper.GenerateSlug(src.option_key);
        if (await _unitOfWork.OptionRepository.ExistsAsync(x => x.slug == slug && entity.slug != slug))
        {
            var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<OptionDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}

        entity.slug = slug;
        entity.updated_at = DateTime.UtcNow;
        entity.option_value = src.option_value;
        entity.option_key = src.option_key;

        _unitOfWork.OptionRepository.Update(entity);
        await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<OptionDto>()
		{
			data = null,
			is_success = true,
			message = "آپشن با موفقیت ویرایش شد.",
			response_code = 204
		});
	}
}
