using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Mvc;
using RaelState.Models;

namespace RaelState.Controllers.Admin;
[Route("api/option")]
[ApiController]
public class OptionController(IUnitOfWork unitOfWork) : ControllerBase
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
        var data = _unitOfWork.OptionRepository.GetPaginated(filter);
        return Ok(data);
    }

    [HttpGet]
    [Route("get-all-cities")]
    public async Task<IActionResult> GetAll()
    {
        var data = await _unitOfWork.OptionRepository.GetAll();
        if (data.Count() == 0)
            return Ok(new List<OptionDto>());
        return Ok(data.Select(x => new OptionDto()
        {
            id = x.id,
            created_at = x.created_at,
            updated_at = x.updated_at,
            slug = x.slug,
            option_key = x.option_key,
            option_value = x.option_value,
        }).ToList());
    }

    [HttpGet]
    [Route("option-detail/{slug}")]
    public async Task<IActionResult> Detail([FromRoute] string slug)
    {
        var entity = await _unitOfWork.OptionRepository.Get(slug);
        if (entity == null)
            return NotFound();
        return Ok(new OptionDto()
        {
            id = entity.id,
            created_at = entity.created_at,
            updated_at = entity.updated_at,
            slug = entity.slug,
            option_key = entity.option_key,
            option_value = entity.option_value,
        });
    }


    [HttpGet]
    [Route("get-option/{id}")]
    public async Task<IActionResult> Get([FromRoute] long id)
    {
        var entity = await _unitOfWork.OptionRepository.Get(id);
        if (entity == null)
            return NotFound();
        return Ok(new OptionDto()
        {
			id = entity.id,
			created_at = entity.created_at,
			updated_at = entity.updated_at,
			slug = entity.slug,
			option_key = entity.option_key,
			option_value = entity.option_value,
		});
    }

    [HttpPost]
    [Route("option-delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] long id)
    {
        var entity = await _unitOfWork.OptionRepository.Get(id);
        if (entity == null)
            return NotFound();
        _unitOfWork.OptionRepository.Remove(entity);
        await _unitOfWork.CommitAsync();
        return NoContent();
    }

    [HttpPost]
    [Route("create-option")]
    public async Task<IActionResult> Create([FromForm] OptionDto src)
    {
        if (!ModelState.IsValid)
        {
            var error = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            return BadRequest(error);
        }
        if (await _unitOfWork.OptionRepository.ExistsAsync(x => x.option_key == src.option_key))
        {
            var error = "مقدار کلید تکراریست لطفا تغییر دهید.";
            return BadRequest(error);
        }
        var slug = src.slug ?? SlugHelper.GenerateSlug(src.option_key);
        if (await _unitOfWork.OptionRepository.ExistsAsync(x => x.slug == slug))
        {
            var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
            return BadRequest(error);
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
        return Created();
    }

    [HttpPost]
    [Route("edit-option")]
    public async Task<IActionResult> Edit([FromForm] OptionDto src)
    {
        if (!ModelState.IsValid)
        {
            var error = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            return BadRequest(error);
        }
        var entity = await _unitOfWork.OptionRepository.Get(src.id);
        if (entity == null)
            return NotFound();

        if (await _unitOfWork.OptionRepository.ExistsAsync(x => x.option_key == src.option_key && entity.option_key != src.option_key))
        {
            var error = "مقدار کلید تکراریست لطفا تغییر دهید.";
            return BadRequest(error);
        }
        var slug = src.slug ?? SlugHelper.GenerateSlug(src.option_key);
        if (await _unitOfWork.OptionRepository.ExistsAsync(x => x.slug == slug && entity.slug != slug))
        {
            var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
            return BadRequest(error);
        }

        entity.slug = slug;
        entity.updated_at = DateTime.UtcNow;
        entity.option_value = src.option_value;
        entity.option_key = src.option_key;

        _unitOfWork.OptionRepository.Update(entity);
        await _unitOfWork.CommitAsync();
        return NoContent();
    }
}
