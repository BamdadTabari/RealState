using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealState.Models;
using System.Linq;

namespace RealState.Controllers.Public;

[Route("api/public/property")]
[ApiController]
public class PublicPropertyController(IUnitOfWork unitOfWork) : ControllerBase
{
	[HttpGet]
	[Route("list")]
	public async Task<IActionResult> Index([FromQuery] string? search_term,
		[FromQuery] SortByEnum sort_by = SortByEnum.CreationDate,
		[FromQuery] int page = 1,
		[FromQuery] int page_size = 10,
		[FromQuery] long? city_id = null, [FromQuery] long? category_id = null,
		[FromQuery] bool? is_rent = null, [FromQuery] List<long>? option_list = null,
		[FromQuery] int? floor = null,
		[FromQuery] decimal? fromMeterage = null, [FromQuery] decimal? toMeterage = null,
		[FromQuery] decimal? fromPrice = null, [FromQuery] decimal? toPrice = null)
	{
		var filter = new DefaultPaginationFilter(page, page_size)
		{
			Keyword = search_term,
			SortBy = sort_by,
			BoolFilter = true
		};
		var data = unitOfWork.PropertyRepository.GetPaginated(filter, null);

		if (fromMeterage != null && toMeterage != null)
			data.Data = data.Data.Where(x => x.meterage >= fromMeterage && x.meterage <= toMeterage).ToList();

		if (floor != null)
			data.Data = data.Data.Where(x => x.property_floor == floor).ToList();

		if (city_id != null)
			data.Data = data.Data.Where(x => x.city_id == city_id).ToList();

		if (category_id != null)
			data.Data = data.Data.Where(x => x.category_id == category_id).ToList();

		if (is_rent is true)
		{
			data.Data = data.Data.Where(x => x.type_enum == TypeEnum.Rental).ToList();
			if (fromPrice != null && toPrice != null)
				data.Data = data.Data.Where(x => x.mortgage_price >= fromPrice && x.mortgage_price <= toPrice).ToList();
		}

		if (is_rent is false)
		{
			data.Data = data.Data.Where(x => x.type_enum == TypeEnum.Sell).ToList();
			if (fromPrice != null && toPrice != null)
				data.Data = data.Data.Where(x => x.sell_price >= fromPrice && x.sell_price <= toPrice).ToList();
		}

		if (option_list != null)
			data.Data = data.Data
				.Where(x => option_list.Equals(x.property_facility_properties.Select(x => x.id).ToList())).ToList();

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
		var entity = await unitOfWork.PropertyRepository.Get(slug);
		if (entity == null)
			return NotFound(new ResponseDto<PropertyDto>()
			{
				data = null,
				is_success = false,
				message = "ملک با این نامک پیدا نشد",
				response_code = 404
			});
		var agency = await unitOfWork.AgencyRepository.FindSingle(x => x.user_id == entity.owner_id);
		return Ok(new ResponseDto<PropertyDto>()
		{
			data = new PropertyDto()
			{
				agency_mobile = agency.mobile,
				agency_phone = agency.phone,
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
				property_facility_properties = entity.property_facility_properties == null
					? new List<PropertyFacilityPropertyDto>()
					: entity.property_facility_properties.Select(p => new PropertyFacilityPropertyDto()
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
				images = entity.gallery == null
					? new List<PropertyGalleryDto>()
					: entity.gallery.Select(pg => new PropertyGalleryDto()
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
			},
			is_success = true,
			message = "ok",
			response_code = 200
		});
	}

	[HttpGet]
	[Route("similar/{slug}")]
	public async Task<IActionResult> GetSimilarProperties([FromRoute] string slug)
	{
		var property = await unitOfWork.PropertyRepository.Get(slug);
		if (property == null)
			return NotFound(new ResponseDto<PropertyDto>()
			{
				data = null,
				is_success = false,
				message = "ملک با این نامک پیدا نشد",
				response_code = 404
			});

		var data = await unitOfWork.PropertyRepository
			.GetSimilarProperties(take: 5, categoryId: property.category_id, cityId: property.city_id, typeEnum: property.type_enum);

		return Ok(new ResponseDto<List<PropertyDto>>()
		{
			data = data.Select(entity=> new PropertyDto()
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
				images = entity.gallery == null
					? new List<PropertyGalleryDto>()
					: entity.gallery.Select(pg => new PropertyGalleryDto()
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

			}).ToList()
		});
	}
}