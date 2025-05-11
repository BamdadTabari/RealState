using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaelState.Models;
using RealState.Models;

namespace RaelState.Controllers;
[Route("api/role")]
[ApiController]
[Authorize(Roles = "Admin,MainAdmin")]
public class RoleController(IUnitOfWork unitOfWork) : ControllerBase
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
        var data = _unitOfWork.RoleRepository.GetPaginated(filter);
		return Ok(new ResponseDto<PaginatedList<Role>>()
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
        var data = await _unitOfWork.RoleRepository.GetAll();
        if (data.Count() == 0)
			return NotFound(new ResponseDto<List<RoleDto>>()
			{
				data = new List<RoleDto>(),
				is_success = true,
				message = "مقدار نقش در دیتابیس وجود ندارد.",
				response_code = 200
			});
		return Ok(new ResponseDto<List<RoleDto>>()
        {
            data = data.Select(entity => new RoleDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				title = entity.title,
			}).ToList()
		});
    }

    [HttpGet]
    [Route("read/{slug}")]
    public async Task<IActionResult> Detail([FromRoute] string slug)
    {
        var entity = await _unitOfWork.RoleRepository.GetRoleBySlug(slug);
        if (entity == null)
			return NotFound(new ResponseDto<RoleDto>()
			{
				data = new RoleDto(),
				is_success = false,
				message = "نقش با این slug پیدا نشد",
				response_code = 404
			});
		return Ok(new ResponseDto<RoleDto>()
        {
            data = new RoleDto()
			{

				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				title = entity.title
			},
            is_success = true,
            message = "",
            response_code = 200
		});
    }


    [HttpGet]
    [Route("get/{id}")]
    public async Task<IActionResult> Get([FromRoute] long id)
    {
        var entity = await _unitOfWork.RoleRepository.GetRole(id);
        if (entity == null)
			return NotFound(new ResponseDto<RoleDto>()
			{
				data = new RoleDto(),
				is_success = false,
				message = "نقش با این ایدی پیدا نشد",
				response_code = 404
			});
		return Ok(new ResponseDto<RoleDto>()
        {
            data = new RoleDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				title = entity.title
			},
            is_success=true,
            message = "",
            response_code = 200
		});
    }

    [HttpPost]
    [Route("delete")]
    public async Task<IActionResult> Delete([FromForm] long id)
    {
        var entity = await _unitOfWork.RoleRepository.GetRole(id);
        if (entity == null)
			return NotFound(new ResponseDto<RoleDto>()
			{
				data = new RoleDto(),
				is_success = false,
				message = "نقش با این ایدی پیدا نشد",
				response_code = 404
			});
		_unitOfWork.RoleRepository.Remove(entity);
        await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<RoleDto>()
		{
			data = new RoleDto(),
			is_success = true,
			message = "نقش با موفقیت حذف شد",
			response_code = 204
		});
	}

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create([FromForm] RoleDto src)
    {
        if (!ModelState.IsValid)
        {
            var error = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<RoleDto>()
			{
				data = new RoleDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
        if (await _unitOfWork.RoleRepository.ExistsAsync(x => x.title == src.title))
        {
            var error = "مقدار نام تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<RoleDto>()
			{
				data = new RoleDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
        var slug = src.slug ?? SlugHelper.GenerateSlug(src.title);
        if (await _unitOfWork.RoleRepository.ExistsAsync(x => x.slug == slug))
        {
            var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<RoleDto>()
			{
				data = new RoleDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}

        await _unitOfWork.RoleRepository.AddAsync(new Role()
        {
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow,
            slug = slug,
            title = src.title,
        });
        await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<RoleDto>()
		{
			data = new RoleDto(),
			is_success = true,
			message = "نقش با موفقیت ایجاد شد.",
			response_code = 201
		});
	}

    [HttpPost]
    [Route("edit")]
    public async Task<IActionResult> Edit([FromForm] RoleDto src)
    {
        if (!ModelState.IsValid)
        {
            var error = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<RoleDto>()
			{
				data = new RoleDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
        var entity = await _unitOfWork.RoleRepository.GetRole(src.id);
        if (entity == null)
			return NotFound(new ResponseDto<RoleDto>()
			{
				data = new RoleDto(),
				is_success = false,
				message = "نقش با این ایدی پیدا نشد",
				response_code = 404
			});

		if (await _unitOfWork.RoleRepository.ExistsAsync(x => x.title == src.title && entity.title != src.title))
        {
            var error = "مقدار نام تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<RoleDto>()
			{
				data = new RoleDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
        var slug = src.slug ?? SlugHelper.GenerateSlug(src.title);
        if (await _unitOfWork.RoleRepository.ExistsAsync(x => x.slug == slug && entity.slug != slug))
        {
            var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<RoleDto>()
			{
				data = new RoleDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}

        entity.slug = slug;
        entity.updated_at = DateTime.UtcNow;
        entity.title = src.title;
        
        _unitOfWork.RoleRepository.Update(entity);
        await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<RoleDto>()
		{
			data = new RoleDto(),
			is_success = true,
			message = "نقش با موفقیت ویرایش شد.",
			response_code = 204
		});
	}
}
