using DataLayer;
using Microsoft.AspNetCore.Mvc;
using RealState.Models;

namespace RealState.Controllers.Public;
[Route("api/public/property-situation")]
[ApiController]
public class PublicPropertySituationController(IUnitOfWork unitOfWork) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	[HttpGet]
	[Route("all")]
	public async Task<IActionResult> GetAll()
	{
		var data = await _unitOfWork.PropertySituationRepository.GetAll();
		if (data.Count() == 0)
			return Ok(new ResponseDto<List<PropertySituationDto>>()
			{
				data = null,
				is_success = true,
				message = "مقدار وضعیت ملک در دیتابیس وجود ندارد.",
				response_code = 200
			});
		return Ok(new ResponseDto<List<PropertySituationDto>>()
		{
			data = data.Select(entity => new PropertySituationDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				name = entity.name,
				properties = entity.properties == null ? new List<PropertyDto>() :
				entity.properties.Select(y => new PropertyDto()
				{
					id = y.id,
					created_at = y.created_at,
					updated_at = y.updated_at,
					slug = y.slug,
					name = y.name,
					address = y.address,
					bed_room_count = y.bed_room_count,
					category_id = y.category_id,
					city_id = y.city_id,
					city_province_full_name = y.city_province_full_name,
					code = y.code,
					description = y.description,
					//gallery = y.gallery,
					is_for_sale = y.is_for_sale,
					meterage = y.meterage,
					mortgage_price = y.meterage,
					property_age = y.property_age,
					property_floor = y.property_floor,
					rent_price = y.rent_price,
					sell_price = y.sell_price,
					situation_id = y.situation_id,
					state_enum = y.state_enum,
					type_enum = y.type_enum,
				}).ToList()
			}).ToList(),
			is_success = true,
			message = "",
			response_code = 200
		});
	}

}
