using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Mvc;
using RaelState.Assistant;
using RaelState.Models;
using RealState.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RaelState.Controllers;
[Route("api/blog-category")]
[ApiController]
public class BlogCategoryController(IUnitOfWork unitOfWork) : ControllerBase
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
        var data = _unitOfWork.BlogCategoryRepository.GetPaginated(filter);
		return Ok(new ResponseDto<PaginatedList<BlogCategory>>()
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
        var data = await _unitOfWork.BlogCategoryRepository.GetAll();
        if (data.Count() == 0)
		    return NotFound(new ResponseDto<List<BlogCategoryDto>> ()
		    {
			    data = new List<BlogCategoryDto>(),
			    is_success = false,
			    message = "هیچ مقداری در دیتابیس وجود ندارد",
			    response_code = 404
		    });
		return Ok(new ResponseDto<List<BlogCategoryDto>>()
		{
			data = data.Select(x => new BlogCategoryDto()
			{
				id = x.id,
				name = x.name,
				description = x.description,
				created_at = x.created_at,
				updated_at = x.updated_at,
				slug = x.slug,
				blogs = x.blogs == null ? [] :
			x.blogs.Select(y => new BlogDto()
			{
				id = y.id,
				created_at = y.created_at,
				updated_at = y.updated_at,
				slug = y.slug,
				blog_category_id = y.blog_category_id,
				blog_text = y.blog_text,
				image_alt = y.image_alt,
				image = y.image,
				keyWords = y.keywords,
				name = y.name,
				description = y.description,
				show_blog = y.show_blog,
			}
			).ToList()
			}).ToList(),
			is_success = true,
			message = "",
			response_code = 200
		});
    }

    [HttpGet]
    [Route("read/{slug}")]
    public async Task<IActionResult> Detail([FromRoute] string slug)
    {
        var entity = await _unitOfWork.BlogCategoryRepository.Get(slug);
        if (entity == null)
			return NotFound(new ResponseDto<BlogCategoryDto>()
			{
				data = new BlogCategoryDto(),
				is_success = false,
				message = "دسته بندی با این slug پیدا نشد",
				response_code = 404
			});
		return Ok(new ResponseDto<BlogCategoryDto> (){ 
            data = new BlogCategoryDto()
			{
				id = entity.id,
				name = entity.name,
				description = entity.description,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				blogs = entity.blogs == null ? [] :
			entity.blogs.Select(y => new BlogDto()
			{
				id = y.id,
				created_at = y.created_at,
				updated_at = y.updated_at,
				slug = y.slug,
				blog_category_id = y.blog_category_id,
				blog_text = y.blog_text,
				image_alt = y.image_alt,
				image = y.image,
				keyWords = y.keywords,
				name = y.name,
				description = y.description,
				show_blog = y.show_blog,
			}
			).ToList()
			},
            is_success= true,
            message = "",
            response_code = 200
        });
    }


    [HttpGet]
    [Route("get/{id}")]
    public async Task<IActionResult> Get([FromRoute] long id)
    {
        var entity = await _unitOfWork.BlogCategoryRepository.Get(id);
        if (entity == null)
			return NotFound(new ResponseDto<BlogCategoryDto>()
			{
				data = new BlogCategoryDto(),
				is_success = false,
				message = "دسته بت=ندی با این ایدی پیدا نشد",
				response_code = 404
			});
		return Ok(new ResponseDto<BlogCategoryDto>()
		{
			data = new BlogCategoryDto()
			{
				id = entity.id,
				name = entity.name,
				description = entity.description,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				blogs = entity.blogs == null ? [] :
			entity.blogs.Select(y => new BlogDto()
			{
				id = y.id,
				created_at = y.created_at,
				updated_at = y.updated_at,
				slug = y.slug,
				blog_category_id = y.blog_category_id,
				blog_text = y.blog_text,
				image_alt = y.image_alt,
				image = y.image,
				keyWords = y.keywords,
				name = y.name,
				description = y.description,
				show_blog = y.show_blog,
			}
			).ToList()
			},
			is_success= true,
			message = "",
			response_code = 200
		});
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] long id)
    {
        var entity = await _unitOfWork.BlogCategoryRepository.Get(id);
        if (entity == null)
            return NotFound(new ResponseDto<BlogCategoryDto>()
            {
                data = new BlogCategoryDto(),
                is_success = false,
                message = "دسته بندی با این ایدی پیدا نشد.",
                response_code = 404
            });
        _unitOfWork.BlogCategoryRepository.Remove(entity);
        await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<BlogCategoryDto>()
		{
			data = new BlogCategoryDto(),
			is_success = true,
			message = "دسته بندی با موفقیت حذف شد",
			response_code = 204
		});
	}

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create([FromForm] BlogCategoryDto src)
    {
        if (!ModelState.IsValid)
        {
            var error = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<BlogCategoryDto>()
			{
				data = new BlogCategoryDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
        if (await _unitOfWork.BlogCategoryRepository.ExistsAsync(x => x.name == src.name))
        {
            var error = "مقدار نام تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<BlogCategoryDto>()
			{
				data = new BlogCategoryDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
        var slug = src.slug ?? SlugHelper.GenerateSlug(src.name);
        if (await _unitOfWork.BlogCategoryRepository.ExistsAsync(x => x.slug == slug))
        {
            var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<BlogCategoryDto>()
			{
				data = new BlogCategoryDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}

        await _unitOfWork.BlogCategoryRepository.AddAsync(new BlogCategory()
        {
            name = src.name,
            slug = slug,
            description = src.description,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow,
        });
        await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<BlogCategoryDto>()
		{
			data = new BlogCategoryDto(),
			is_success = true,
			message = "دسته بندی با موفقیت ایجاد شد.",
			response_code = 201
		});
	}

    [HttpPost]
    [Route("edit")]
    public async Task<IActionResult> Edit([FromForm] BlogCategoryDto src)
    {
        if (!ModelState.IsValid)
        {
            var error = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<BlogCategoryDto>()
			{
				data = new BlogCategoryDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
        var entity = await _unitOfWork.BlogCategoryRepository.Get(src.id);
        if (entity == null)
			return NotFound(new ResponseDto<BlogCategoryDto>()
			{
				data = new BlogCategoryDto(),
				is_success = false,
				message = "دسته بندی با این ایدی پیدا نشد.",
				response_code = 404
			});

		if (await _unitOfWork.BlogCategoryRepository.ExistsAsync(x => x.name == src.name && entity.name != src.name))
        {
            var error = "مقدار نام تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<BlogCategoryDto>()
			{
				data = new BlogCategoryDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
        var slug = src.slug ?? SlugHelper.GenerateSlug(src.name);
        if (await _unitOfWork.BlogCategoryRepository.ExistsAsync(x => x.slug == slug && entity.slug != slug))
        {
            var error = "مقدار نامک تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<BlogCategoryDto>()
			{
				data = new BlogCategoryDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}

        entity.slug = slug;
        entity.name = src.name;
        entity.description = src.description;
        entity.updated_at = DateTime.UtcNow;
        _unitOfWork.BlogCategoryRepository.Update(entity);
        await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<BlogCategoryDto>()
		{
			data = new BlogCategoryDto(),
			is_success = true,
			message = "دسته بندی با موفقیت ویرایش شد",
			response_code = 204
		});
	}
}
