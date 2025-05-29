using DataLayer;
using DataLayer.Assistant.Enums;
using DataLayer.EntityFramework.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaelState.Models;
using RealState.Models;

namespace RealState.Controllers.Admin;
[Route("api/contact")]
[ApiController]
[Authorize(Roles = "Admin,MainAdmin")]
public class ContactController(IUnitOfWork unitOfWork) : ControllerBase
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
		var data = _unitOfWork.ContactRepository.GetPaginated(filter);
		return Ok(new ResponseDto<PaginatedList<Contact>>()
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
		var data = await _unitOfWork.ContactRepository.GetAll();
		if (data.Count() == 0)
			return Ok(new ResponseDto<List<ContactDto>>()
			{
				data = null,
				is_success = true,
				message = "مقدار شهر در دیتابیس وجود ندارد.",
				response_code = 200
			});
		return Ok(new ResponseDto<List<ContactDto>>()
		{
			data = data.Select(entity => new ContactDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				mobile = entity.mobile,
				message = entity.message,
				full_name = entity.full_name,
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
		var entity = await _unitOfWork.ContactRepository.Get(slug);
		if (entity == null)
			return NotFound(new ResponseDto<ContactDto>()
			{
				data = null,
				is_success = false,
				message = "شهر با این slug پیدا نشد",
				response_code = 404
			});
		return Ok(new ResponseDto<ContactDto>()
		{
			data = new ContactDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				full_name=entity.full_name,
				message = entity.message,
				mobile = entity.mobile
			},
			is_success = true,
			message = "",
			response_code = 200,
		});
	}


}
