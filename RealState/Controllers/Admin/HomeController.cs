using DataLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealState.Models;

namespace RealState.Controllers.Admin;
[Route("api/home")]
[ApiController]
[Authorize(Roles = "Admin,MainAdmin")]
public class HomeController(IUnitOfWork unitOfWork) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	[HttpGet]
	[Route("dashboard")]
	public IActionResult Index()
	{
		var userCount = _unitOfWork.UserRepository.Count();
		var activeUsers = _unitOfWork.UserRepository.Count(x=>x.is_active == true);
		var deActiveUsers = _unitOfWork.UserRepository.Count(x => x.is_active == false);

		var propertyCount = _unitOfWork.PropertyRepository.Count();
		var pendingPropertiesCount = _unitOfWork.PropertyRepository.Count(x => x.state_enum == AdStatusEnum.NotConfirmed);
		var archiveProperties = _unitOfWork.PropertyRepository.Count(x => x.state_enum == AdStatusEnum.Archived);

		var ticketCount = _unitOfWork.TicketRepository.Count();
		var openTicketCount = _unitOfWork.TicketRepository.Count(x=>x.status == TicketStatus.Open);
		var repliedTicketCount = _unitOfWork.TicketRepository.Count(x => x.status == TicketStatus.Answered);

		var orders = _unitOfWork.OrderRepository.Count();
		var failedOrders = _unitOfWork.OrderRepository.Count(x=>x.order_status == DataLayer.Assistant.Enums.OrderStatusEnum.Fail);
		var successOrders = _unitOfWork.OrderRepository.Count(x => x.order_status == DataLayer.Assistant.Enums.OrderStatusEnum.Success);

		return Ok(new ResponseDto<DashboardDto>()
		{
			data = new DashboardDto()
			{
				users = userCount,
				active_users = activeUsers,
				de_active_users = deActiveUsers,

				pending_properties = pendingPropertiesCount,
				properties = propertyCount,
				archived_properties = archiveProperties,

				tickets = ticketCount,
				open_tickets = ticketCount,
				replied_tickets = repliedTicketCount,

				orders = orders,
				failed_orders = failedOrders,
				success_orders = successOrders,
			},
			is_success = true,
			message = "",
			response_code = 200
		});
	}
}
