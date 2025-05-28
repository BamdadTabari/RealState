using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealState.Models;

namespace RealState.Controllers.Public;
[Route("api/public/property-category")]
[ApiController]
public class PublicPropertyCategoryController(IUnitOfWork unitOfWork) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	[HttpGet]
	[Route("all")]
	public async Task<IActionResult> Index()
	{
		var data = await _unitOfWork.PropertyCategoryRepository.GetAll();
		return Ok(new ResponseDto<List<PropertyCategory>>()
		{
			data = data,
			is_success = true,
			message = "",
			response_code = 200
		});
	}
}
