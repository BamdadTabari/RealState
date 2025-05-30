﻿using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealState.Models;

namespace RealState.Controllers.Public;
[Route("api/public/property")]
[ApiController]
public class PublicPropertyController(IUnitOfWork unitOfWork) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	[HttpGet]
	[Route("list")]
	public async Task<IActionResult> Index(string? search_term,
		SortByEnum sort_by = SortByEnum.CreationDate,
		int page = 1,
		int page_size = 10)
	{
		var filter = new DefaultPaginationFilter(page, page_size)
		{
			Keyword = search_term,
			SortBy = sort_by,
			BoolFilter = true
		};
		var data = _unitOfWork.PropertyRepository.GetPaginated(filter, null);
		return Ok(new ResponseDto<PaginatedList<Property>>()
		{
			data = data,
			is_success = true,
			message = "",
			response_code = 200
		});
	}

	[HttpGet]
	[Route("read/{slug}")]
	public async Task<IActionResult> Get([FromRoute] string slug)
	{
		var entity = await _unitOfWork.PropertyRepository.Get(slug);
		if (entity == null)
			return NotFound(new ResponseDto<PropertyDto>()
			{
				data = null,
				is_success = false,
				message = "ملک با این نامک پیدا نشد",
				response_code = 404
			});
		return Ok(new PropertyDto()
		{
			id = entity.id,
			created_at = entity.created_at,
			updated_at = entity.updated_at,
			slug = slug,
			address = entity.address,
			bed_room_count = entity.bed_room_count,
			category_id = entity.category_id,
			city_id = entity.city_id,
			code = entity.code,
			description = entity.description,
			city_province_full_name = entity.city_province_full_name,
			is_for_sale = entity.is_for_sale,
			meterage = entity.meterage,
			mortgage_price = entity.mortgage_price,
			name = entity.name,
			owner_id = entity.owner_id,
			property_age = entity.property_age,
			property_floor = entity.property_floor,
			rent_price = entity.rent_price,
			sell_price = entity.sell_price,
			situation_id = entity.situation_id,
			state_enum = entity.state_enum,
			type_enum = entity.type_enum,
			property_category = new PropertyCategoryDto()
			{
				id = entity.property_category.id,
				created_at = entity.property_category.created_at,
				updated_at = entity.property_category.updated_at,
				slug = entity.property_category.slug,
				name = entity.property_category.name,
			},
			property_facility_properties = entity.property_facility_properties == null ? new List<PropertyFacilityPropertyDto>() :
			entity.property_facility_properties.Select(p => new PropertyFacilityPropertyDto()
			{
				id = p.id,
				created_at = p.created_at,
				updated_at = p.updated_at,
				slug = p.slug,
				property_facility_id = p.property_facility_id,
				property_id = p.property_facility_id,
				property_facility = new PropertyFacilityDto()
				{
					id = p.property_facility.id,
					created_at = p.property_facility.created_at,
					updated_at = p.property_facility.updated_at,
					slug = p.property_facility.slug,
					name = p.property_facility.name,
				}
			}).ToList(),
			gallery = entity.gallery == null ? new List<PropertyGalleryDto>() :
			entity.gallery.Select(pg => new PropertyGalleryDto()
			{
				id = pg.id,
				created_at = pg.created_at,
				updated_at = pg.updated_at,
				slug = pg.slug,
				picture = pg.picture,
				alt = pg.alt,
				property_id = pg.property_id,
			}).ToList(),
			video = entity.video,
			video_caption = entity.video_caption,
		});
	}
}
