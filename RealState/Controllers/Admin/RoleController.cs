using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Mvc;
using RaelState.Assistant;
using RaelState.Models;

namespace RaelState.Controllers;
[Route("api/role")]
[ApiController]
public class RoleController(IUnitOfWork unitOfWork) : ControllerBase
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
        var data = _unitOfWork.RoleRepository.GetPaginated(filter);
        return Ok(data);
    }

    [HttpGet]
    [Route("get-all-roles")]
    public async Task<IActionResult> GetAll()
    {
        var data = await _unitOfWork.RoleRepository.GetAll();
        if (data.Count() == 0)
            return Ok(new List<RoleDto>());
        return Ok(data.Select(x => new RoleDto()
        {
            id = x.id,
            created_at = x.created_at,
            updated_at = x.updated_at,
            slug = x.slug,
            title = x.title,
        }).ToList());
    }

    [HttpGet]
    [Route("role-detail/{slug}")]
    public async Task<IActionResult> Detail([FromRoute] string slug)
    {
        var entity = await _unitOfWork.RoleRepository.GetRoleBySlug(slug);
        if (entity == null)
            return NotFound();
        return Ok(new RoleDto()
        {

            id = entity.id,
            created_at = entity.created_at,
            updated_at = entity.updated_at,
            slug = entity.slug,
            title = entity.title
        });
    }


    [HttpGet]
    [Route("get-role/{id}")]
    public async Task<IActionResult> Get([FromRoute] long id)
    {
        var entity = await _unitOfWork.RoleRepository.GetRole(id);
        if (entity == null)
            return NotFound();
        return Ok(new RoleDto()
        {
            id = entity.id,
            created_at = entity.created_at,
            updated_at = entity.updated_at,
            slug = entity.slug,
            title = entity.title
        });
    }

    [HttpPost]
    [Route("role-delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] long id)
    {
        var entity = await _unitOfWork.RoleRepository.GetRole(id);
        if (entity == null)
            return NotFound();
        _unitOfWork.RoleRepository.Remove(entity);
        await _unitOfWork.CommitAsync();
        return NoContent();
    }

    [HttpPost]
    [Route("create-role")]
    public async Task<IActionResult> Create([FromForm] RoleDto src)
    {
        if (!ModelState.IsValid)
        {
            var error = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            return BadRequest(error);
        }
        if (await _unitOfWork.RoleRepository.ExistsAsync(x => x.title == src.title))
        {
            var error = "مقدار نام تکراریست لطفا تغییر دهید.";
            return BadRequest(error);
        }
        var slug = src.slug ?? SlugHelper.GenerateSlug(src.title);
        if (await _unitOfWork.RoleRepository.ExistsAsync(x => x.slug == slug))
        {
            var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
            return BadRequest(error);
        }

        await _unitOfWork.RoleRepository.AddAsync(new Role()
        {
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow,
            slug = slug,
            title = src.title,
        });
        await _unitOfWork.CommitAsync();
        return Created();
    }

    [HttpPost]
    [Route("edit-role")]
    public async Task<IActionResult> Edit([FromForm] RoleDto src)
    {
        if (!ModelState.IsValid)
        {
            var error = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            return BadRequest(error);
        }
        var entity = await _unitOfWork.RoleRepository.GetRole(src.id);
        if (entity == null)
            return NotFound();

        if (await _unitOfWork.RoleRepository.ExistsAsync(x => x.title == src.title && entity.title != src.title))
        {
            var error = "مقدار نام تکراریست لطفا تغییر دهید.";
            return BadRequest(error);
        }
        var slug = src.slug ?? SlugHelper.GenerateSlug(src.title);
        if (await _unitOfWork.RoleRepository.ExistsAsync(x => x.slug == slug && entity.slug != slug))
        {
            var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
            return BadRequest(error);
        }

        entity.slug = slug;
        entity.updated_at = DateTime.UtcNow;
        entity.title = src.title;
        
        _unitOfWork.RoleRepository.Update(entity);
        await _unitOfWork.CommitAsync();
        return NoContent();
    }
}
