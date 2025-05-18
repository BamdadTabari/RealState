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
		var propertyCount = _unitOfWork.PropertyRepository.Count();
		var ticketCount = _unitOfWork.TicketRepository.Count();
		var pendingPropertiesCount = _unitOfWork.PropertyRepository.Count(x=>x.state_enum == AdStatusEnum.NotConfirmed);

		return Ok(new ResponseDto<DashboardDto>()
		{
			data = new DashboardDto()
			{
				users = userCount,
				pending_properties = pendingPropertiesCount,
				properties = propertyCount,
				tickets = ticketCount
			},
			is_success = true,
			message = "",
			response_code = 200
		});
	}
}
