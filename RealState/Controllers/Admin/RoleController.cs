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
    [HasPermission("Role.Index")]
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
    [HasPermission("Role.GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var data = await _unitOfWork.RoleRepository.GetAll();
        if (data.Count() == 0)
            return Ok(new List<RoleDto>());
        return Ok(data.Select(x => new RoleDto()
        {
            Id = x.Id,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
            Slug = x.Slug,
            Title = x.Title,
            RolePermissions = x.RolePermissions == null ? new List<RolePermissionDto>():
            x.RolePermissions.Select(rp => new RolePermissionDto()
            {
                Id =rp.Id,
                CreatedAt = rp.CreatedAt,
                UpdatedAt =rp.UpdatedAt,
                Slug = rp.Slug,
                PermissionId = rp.PermissionId,
                RoleId = rp.RoleId,
            }).ToList()
        }).ToList());
    }

    [HttpGet]
    [Route("role-detail/{slug}")]
    [HasPermission("Role.Detail")]
    public async Task<IActionResult> Detail([FromRoute] string slug)
    {
        var entity = await _unitOfWork.RoleRepository.GetRoleBySlug(slug);
        if (entity == null)
            return NotFound();
        return Ok(new RoleDto()
        {

            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Slug = entity.Slug,
            Title = entity.Title,
            RolePermissions = entity.RolePermissions == null ? new List<RolePermissionDto>() :
            entity.RolePermissions.Select(rp => new RolePermissionDto()
            {
                Id = rp.Id,
                CreatedAt = rp.CreatedAt,
                UpdatedAt = rp.UpdatedAt,
                Slug = rp.Slug,
                PermissionId = rp.PermissionId,
                RoleId = rp.RoleId,
            }).ToList()
        });
    }


    [HttpGet]
    [Route("get-role/{id}")]
    [HasPermission("Role.Get")]
    public async Task<IActionResult> Get([FromRoute] long id)
    {
        var entity = await _unitOfWork.RoleRepository.GetRole(id);
        if (entity == null)
            return NotFound();
        return Ok(new RoleDto()
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Slug = entity.Slug,
            Title = entity.Title,
            RolePermissions = entity.RolePermissions == null ? new List<RolePermissionDto>() :
            entity.RolePermissions.Select(rp => new RolePermissionDto()
            {
                Id = rp.Id,
                CreatedAt = rp.CreatedAt,
                UpdatedAt = rp.UpdatedAt,
                Slug = rp.Slug,
                PermissionId = rp.PermissionId,
                RoleId = rp.RoleId,
            }).ToList()
        });
    }

    [HttpPost]
    [Route("role-delete/{id}")]
    [HasPermission("Role.Delete")]
    public async Task<IActionResult> Delete([FromRoute] long id)
    {
        var entity = await _unitOfWork.RoleRepository.GetRole(id);
        if (entity == null)
            return NotFound();
        _unitOfWork.RoleRepository.Remove(entity);
        await _unitOfWork.CommitAsync();
        return Ok();
    }

    [HttpPost]
    [Route("create-role")]
    [HasPermission("Role.Create")]
    public async Task<IActionResult> Create([FromForm] RoleDto src)
    {
        if (!ModelState.IsValid)
        {
            var error = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            return BadRequest(error);
        }
        if (await _unitOfWork.RoleRepository.ExistsAsync(x => x.Title == src.Title))
        {
            var error = "مقدار نام تکراریست لطفا تغییر دهید.";
            return BadRequest(error);
        }
        var slug = src.Slug ?? SlugHelper.GenerateSlug(src.Title);
        if (await _unitOfWork.RoleRepository.ExistsAsync(x => x.Slug == slug))
        {
            var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
            return BadRequest(error);
        }

        await _unitOfWork.RoleRepository.AddAsync(new Role()
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Slug = slug,
            Title = src.Title,
        });
        await _unitOfWork.CommitAsync();
        return Ok();
    }

    [HttpPost]
    [Route("edit-role")]
    [HasPermission("Role.Edit")]
    public async Task<IActionResult> Edit([FromForm] RoleDto src)
    {
        if (!ModelState.IsValid)
        {
            var error = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            return BadRequest(error);
        }
        var entity = await _unitOfWork.RoleRepository.GetRole(src.Id);
        if (entity == null)
            return NotFound();

        if (await _unitOfWork.RoleRepository.ExistsAsync(x => x.Title == src.Title && entity.Title != src.Title))
        {
            var error = "مقدار نام تکراریست لطفا تغییر دهید.";
            return BadRequest(error);
        }
        var slug = src.Slug ?? SlugHelper.GenerateSlug(src.Title);
        if (await _unitOfWork.RoleRepository.ExistsAsync(x => x.Slug == slug && entity.Slug != slug))
        {
            var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
            return BadRequest(error);
        }

        entity.Slug = slug;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.Title = src.Title;
        
        _unitOfWork.RoleRepository.Update(entity);
        await _unitOfWork.CommitAsync();
        return Ok();
    }
}
