using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaelState.Models;
using RealState.Models;

namespace RealState.Controllers.Admin;
[Route("api/order")]
[ApiController]
[Authorize(Roles = "Admin,MainAdmin")]
public class OrderController(IUnitOfWork unitOfWork) : ControllerBase
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
		var data = _unitOfWork.OrderRepository.GetPaginated(filter);
		return Ok(new ResponseDto<PaginatedList<Order>>()
		{
			data = data,
			is_success = true,
			message = "",
			response_code = 200
		});
	}

	[HttpGet]
	[Route("read/{slug}")]
	public async Task<IActionResult> Detail([FromRoute] string slug)
	{
		var entity = await _unitOfWork.OrderRepository.Get(slug);
		if (entity == null)
			return NotFound(new ResponseDto<OrderDto>()
			{
				data = null,
				is_success = false,
				message = "سفارش با این slug پیدا نشد",
				response_code = 404
			});
		return Ok(new ResponseDto<OrderDto>()
		{
			data = new OrderDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				amount = entity.amount,
				authority = entity.authority,
				card_number = entity.card_number,
				date_paid = entity.date_paid,
				email = entity.email,
				mobile = entity.mobile,
				plan_id = entity.plan_id,
				response_message = entity.response_message,
				status = entity.status,
				username = entity.username,
				user_id = entity.user_id,
			},
			is_success = true,
			message = "",
			response_code = 200,
		});
	}
}
