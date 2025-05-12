using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaelState;
using RaelState.Models;
using RealState.Models;

namespace RealState.Controllers.Public;
[Route("api/public/blog")]
[ApiController]
public class PublicBlogController(IUnitOfWork unitOfWork) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	[HttpGet]
	[Route("categories")]
	public async Task<IActionResult> GetAll()
	{
		var data = await _unitOfWork.BlogCategoryRepository.GetAll();
		if (data.Count() == 0)
			return Ok(new ResponseDto<List<BlogCategoryDto>>()
			{
				data = new List<BlogCategoryDto>(),
				is_success = true,
				message = "هیچ مقداری در دیتابیس وجود ندارد",
				response_code = 200
			});
		return Ok(new ResponseDto<List<BlogCategoryDto>>()
		{
			data = data.Select(entity => new BlogCategoryDto()
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
			}).ToList(),
			is_success = true,
			message = "",
			response_code = 200
		});
	}

	[HttpGet]
	[Route("list")]
	public async Task<IActionResult> Index(string? search_term,
		long? category_id,
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
		var data = _unitOfWork.BlogRepository.GetPaginated(filter, category_id);
		return Ok(new ResponseDto<PaginatedList<Blog>>()
		{
			data = data,
			is_success = true,
			message = "",
			response_code = 200
		});
	}
}
