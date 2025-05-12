using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealState.Models;

namespace RealState.Controllers.Public;
[Route("api/public-facility")]
[ApiController]
public class PublicPropertyFacilityController(IUnitOfWork unitOfWork) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	[HttpGet]
	[Route("all")]
	public async Task<IActionResult> GetAll()
	{
		var data = await _unitOfWork.PropertyFacilityRepository.GetAll();
		if (data.Count() == 0)
			return Ok(new ResponseDto<List<PropertyFacilityDto>>()
			{
				data = null,
				is_success = true,
				message = "مقدار ویژگی ملک در دیتابیس وجود ندارد.",
				response_code = 200
			});
		return Ok(new ResponseDto<List<PropertyFacilityDto>>()
		{
			data = data.Select(entity => new PropertyFacilityDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				name = entity.name,
				property_facility_properties = entity.property_facility_properties == null ?
				new List<PropertyFacilityPropertyDto>() :
				entity.property_facility_properties.Select(y => new PropertyFacilityPropertyDto()
				{
					id = y.id,
					created_at = y.created_at,
					updated_at = y.updated_at,
					property_facility_id = y.property_facility_id,
					property_id = y.property_id,
					slug = y.slug,
				}).ToList()
			}).ToList(),
			is_success = true,
			message = "",
			response_code = 200
		});
	}

}
