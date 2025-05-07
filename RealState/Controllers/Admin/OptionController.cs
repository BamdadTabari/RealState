using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Mvc;
using RaelState.Assistant;
using RaelState.Models;

namespace RaelState.Controllers.Admin;
[Route("api/option")]
[ApiController]
public class OptionController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [HttpGet]
    [Route("home")]
    [HasPermission("Option.Index")]
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
    [HasPermission("Option.GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var data = await _unitOfWork.OptionRepository.GetAll();
        if (data.Count() == 0)
            return Ok(new List<OptionDto>());
        return Ok(data.Select(x => new OptionDto()
        {
            Id = x.Id,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
            Slug = x.Slug,
            OptionKey = x.OptionKey,
            OptionValue = x.OptionValue,
        }).ToList());
    }

    [HttpGet]
    [Route("option-detail/{slug}")]
    [HasPermission("Option.Detail")]
    public async Task<IActionResult> Detail([FromRoute] string slug)
    {
        var entity = await _unitOfWork.OptionRepository.Get(slug);
        if (entity == null)
            return NotFound();
        return Ok(new OptionDto()
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Slug = slug,
            OptionKey = entity.OptionKey,
            OptionValue = entity.OptionValue,
        });
    }


    [HttpGet]
    [Route("get-option/{id}")]
    [HasPermission("Option.Get")]
    public async Task<IActionResult> Get([FromRoute] long id)
    {
        var entity = await _unitOfWork.OptionRepository.Get(id);
        if (entity == null)
            return NotFound();
        return Ok(new OptionDto()
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Slug = entity.Slug,
            OptionKey = entity.OptionKey,
            OptionValue = entity.OptionValue,
        });
    }

    [HttpPost]
    [Route("option-delete/{id}")]
    [HasPermission("Option.Delete")]
    public async Task<IActionResult> Delete([FromRoute] long id)
    {
        var entity = await _unitOfWork.OptionRepository.Get(id);
        if (entity == null)
            return NotFound();
        _unitOfWork.OptionRepository.Remove(entity);
        await _unitOfWork.CommitAsync();
        return Ok();
    }

    [HttpPost]
    [Route("create-option")]
    [HasPermission("Option.Create")]
    public async Task<IActionResult> Create([FromForm] OptionDto src)
    {
        if (!ModelState.IsValid)
        {
            var error = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            return BadRequest(error);
        }
        if (await _unitOfWork.OptionRepository.ExistsAsync(x => x.OptionKey == src.OptionKey))
        {
            var error = "مقدار کلید تکراریست لطفا تغییر دهید.";
            return BadRequest(error);
        }
        var slug = src.Slug ?? SlugHelper.GenerateSlug(src.OptionKey);
        if (await _unitOfWork.OptionRepository.ExistsAsync(x => x.Slug == slug))
        {
            var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
            return BadRequest(error);
        }

        await _unitOfWork.OptionRepository.AddAsync(new Option()
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Slug = slug,
            OptionKey = src.OptionKey,
            OptionValue = src.OptionValue
        });
        await _unitOfWork.CommitAsync();
        return Ok();
    }

    [HttpPost]
    [Route("edit-option")]
    [HasPermission("Option.Edit")]
    public async Task<IActionResult> Edit([FromForm] OptionDto src)
    {
        if (!ModelState.IsValid)
        {
            var error = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            return BadRequest(error);
        }
        var entity = await _unitOfWork.OptionRepository.Get(src.Id);
        if (entity == null)
            return NotFound();

        if (await _unitOfWork.OptionRepository.ExistsAsync(x => x.OptionKey == src.OptionKey && entity.OptionKey != src.OptionKey))
        {
            var error = "مقدار کلید تکراریست لطفا تغییر دهید.";
            return BadRequest(error);
        }
        var slug = src.Slug ?? SlugHelper.GenerateSlug(src.OptionKey);
        if (await _unitOfWork.OptionRepository.ExistsAsync(x => x.Slug == slug && entity.Slug != slug))
        {
            var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
            return BadRequest(error);
        }

        entity.Slug = slug;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.OptionValue = src.OptionValue;
        entity.OptionKey = src.OptionKey;

        _unitOfWork.OptionRepository.Update(entity);
        await _unitOfWork.CommitAsync();
        return Ok();
    }
}
