using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaelState.Models;
using RealState.Models;

namespace RealState.Controllers.Public;
[Route("api/public/city")]
[ApiController]
public class PublicCityController(IUnitOfWork unitOfWork) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	[HttpGet]
	[Route("all")]
	public async Task<IActionResult> GetAll()
	{
		var data = await _unitOfWork.CityRepository.GetAll();
		if (data.Count() == 0)
			return Ok(new ResponseDto<List<CityDto>>()
			{
				data = null,
				is_success = true,
				message = "مقدار شهر در دیتابیس وجود ندارد.",
				response_code = 200
			});
		return Ok(new ResponseDto<List<CityDto>>()
		{
			data = data.Select(entity => new CityDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				name = entity.name,
				province_id = entity.province_id,
				province = new ProvinceDto()
				{
					name = entity.province.name,
					id = entity.province.id,
					created_at = entity.province.created_at,
					updated_at = entity.province.updated_at,
					slug = entity.province.slug,
				},
				agency_list = entity.agency_list == null ? new List<AgencyDto>() :
			entity.agency_list.Select(y => new AgencyDto()
			{
				id = y.id,
				created_at = y.created_at,
				updated_at = y.updated_at,
				slug = y.slug,
				agency_name = y.agency_name,
				city_id = y.city_id,
				city_province_full_name = y.city_province_full_name,
				full_name = y.full_name,
				mobile = y.mobile,
				phone = y.phone,
			}).ToList()
			}).ToList(),
			is_success = true,
			message = "",
			response_code = 200
		});
	}
}
