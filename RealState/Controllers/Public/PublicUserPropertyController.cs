using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaelState.Models;
using RealState.Models;

namespace RealState.Controllers.Public;
[Route("api/public/user-property")]
[ApiController]
public class PublicUserPropertyController(IUnitOfWork unitOfWork, JwtTokenService tokenService) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly JwtTokenService _tokenService = tokenService;

	[HttpPost]
	[Route("current-user-id")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<long?> GetCurrentUserId()
	{
		var userId = _tokenService.GetUserIdFromClaims(User) ?? "0";
		var user = await _unitOfWork.UserRepository.GetUser(int.Parse(userId));
		return user?.id;
	}
	[HttpPost]
	[Route("current-user")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<User?> GetCurrentUser()
	{
		var userId = _tokenService.GetUserIdFromClaims(User) ?? "0";
		var user = await _unitOfWork.UserRepository.GetUser(int.Parse(userId));
		return user;
	}
	[HttpGet]
	[Route("list")]
	public async Task<IActionResult> UserProperties(string? search_term,
		SortByEnum sort_by = SortByEnum.CreationDate,
		int page = 1,
		int page_size = 10)
	{
		var userId = await GetCurrentUserId();
		if (userId == null)
			return NotFound(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = "کاربر یافت نشد",
				response_code = 404
			});

		var filter = new DefaultPaginationFilter(page, page_size)
		{
			Keyword = search_term,
			SortBy = sort_by,
		};
		var data = _unitOfWork.PropertyRepository.GetPaginated(filter, userId);
		return Ok(new ResponseDto<PaginatedList<Property>>()
		{
			data = data,
			is_success = true,
			message = "",
			response_code = 200
		});
	}

	[HttpPost]
	[Route("create")]
	public async Task<IActionResult> CreateProperty([FromForm] PropertyDto src)
	{
		var user = await GetCurrentUser();
		if (user == null)
			return NotFound(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = "کاربر یافت نشد. لطفا ابتدا ثبت نام کنید",
				response_code = 404
			});
		if (user.plan_id ==  null)
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = "لطفا ابتدا پلن کاربری خریداری کنید",
				response_code = 404
			});
		if (user.property_count == 0)
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				message = "تعداد املاک پلن شما به پایان رسیده است. لطفا پلن خود را شارژ کنید",
				is_success = false,
				response_code = 400
			});
		if (user.expre_date < DateTime.UtcNow)
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				message = "پلن شما منقضی شده است. لطفا پلن خود را شارژ کنید",
				is_success = false,
				response_code = 400
			});
		var plan = await _unitOfWork.PlanRepository.Get((long)user.plan_id);
		if (plan == null)
			return NotFound(new ResponseDto<PlanDto>()
			{
				data = null,
				is_success = false,
				message = "پلن مورد نظر یافت نشد",
				response_code = 404 
			});
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				   .SelectMany(v => v.Errors)
				   .Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<PropertyDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		
		if (src.gallery_files == null || !src.gallery_files.Any())
		{
			var error = "لطفا گالری تصاویر را پر کنید";
			return BadRequest(new ResponseDto<PropertyDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		if (await _unitOfWork.BlogRepository.ExistsAsync(x => x.name == src.name))
		{
			var error = "ملک با این نام وجود دارد";
			return BadRequest(new ResponseDto<PropertyDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.name);
		if (await _unitOfWork.BlogRepository.ExistsAsync(x => x.slug == slug))
		{
			var error = "ملک با این نامک وجود دارد";
			return BadRequest(new ResponseDto<PropertyDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var city = await _unitOfWork.CityRepository.Get(src.city_id);
		if (city == null)
			return NotFound(new ResponseDto<CityDto>()
			{
				data = null,
				is_success = false,
				message = "شهر با این ایدی پیدا نشد",
				response_code = 404
			});
		var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "images");
		if(!Directory.Exists(uploadPath))
		{
			Directory.CreateDirectory(uploadPath);
		}
		List<string> files = new();
		foreach(var file in src.gallery_files)
		{
			var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
			var imagePath = Path.Combine(uploadPath, fileName);

			// Save Image
			using (var stream = new FileStream(imagePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}
			files.Add(imagePath);
		}
		var code = "";
		do
		{
			code = RandomGenerator.GenerateString(10, AllowedCharacters.Alphanumeric);
		}
		while(await _unitOfWork.PropertyRepository.ExistsAsync(x=>x.code == code));
		
		await _unitOfWork.PropertyRepository.AddAsync(new()
		{
			updated_at = DateTime.Now,
			created_at = DateTime.Now,
			name = src.name,
			slug = slug,
			address = src.address,
			bed_room_count = src.bed_room_count,
			category_id = src.category_id,
			city_id = src.city_id,
			city_province_full_name = city.name + $"({city.province.name})",
			code = code,
			description = src.description,
			is_for_sale = src.is_for_sale,
			meterage = src.meterage,
			mortgage_price = src.meterage,
			owner_id = user.id,
			property_age = src.property_age,
			rent_price = src.rent_price,
			property_floor = src.property_floor,
			sell_price = src.sell_price,
			situation_id = src.situation_id,
			state_enum = src.state_enum,
			type_enum = src.type_enum,
			gallery = files,
			is_active = false,
		});

		await _unitOfWork.CommitAsync();

		user.property_count--;
		return Ok(new ResponseDto<PropertyDto>()
		{
			data = null,
			is_success = true,
			message = "ملک با موفقیت ایجاد شد",
			response_code = 201
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
			entity.property_facility_properties.Select(p => new PropertyFacilityPropertyDto() { 
				id = p.id,
				created_at = p.created_at,
				updated_at = p.updated_at, 
				slug = p.slug,
				property_facility_id = p.property_facility_id,
				property_id = p.property_facility_id,
				property_facility = new PropertyFacilityDto() {
					id = p.property_facility.id,
					created_at = p.property_facility.created_at,
					updated_at = p.property_facility.updated_at,
					slug = p.property_facility.slug,
					name= p.property_facility.name,
				}
			}).ToList(),
		});
	}

	[HttpPost]
	[Route("edit")]
	public async Task<IActionResult> EditProperty([FromForm] PropertyDto src)
	{
		var user = await GetCurrentUser();
		if (user == null)
			return NotFound(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = "کاربر یافت نشد. لطفا ابتدا ثبت نام کنید",
				response_code = 404
			});
		if (user.plan_id == null)
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = "لطفا ابتدا پلن کاربری خریداری کنید",
				response_code = 404
			});
		if (user.property_count == 0)
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				message = "تعداد املاک پلن شما به پایان رسیده است. لطفا پلن خود را شارژ کنید",
				is_success = false,
				response_code = 400
			});
		if (user.expre_date < DateTime.UtcNow)
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				message = "پلن شما منقضی شده است. لطفا پلن خود را شارژ کنید",
				is_success = false,
				response_code = 400
			});
		var plan = await _unitOfWork.PlanRepository.Get((long)user.plan_id);
		if (plan == null)
			return NotFound(new ResponseDto<PlanDto>()
			{
				data = null,
				is_success = false,
				message = "پلن مورد نظر یافت نشد",
				response_code = 404
			});
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				   .SelectMany(v => v.Errors)
				   .Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<PropertyDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}

		var entity = await _unitOfWork.PropertyRepository.Get(src.id);
		if (entity == null)
			return NotFound(new ResponseDto<PropertyDto>()
			{
				data = null,
				is_success = false,
				message = "ملک با این ایدی پیدا نشد",
				response_code = 404
			});
		if (await _unitOfWork.BlogRepository.ExistsAsync(x => x.name == src.name))
		{
			var error = "ملک با این نام وجود دارد";
			return BadRequest(new ResponseDto<PropertyDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.name);
		if (await _unitOfWork.BlogRepository.ExistsAsync(x => x.slug == slug))
		{
			var error = "ملک با این نامک وجود دارد";
			return BadRequest(new ResponseDto<PropertyDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var city = await _unitOfWork.CityRepository.Get(src.city_id);
		if (city == null)
			return NotFound(new ResponseDto<CityDto>()
			{
				data = null,
				is_success = false,
				message = "شهر با این ایدی پیدا نشد",
				response_code = 404
			});
		if(src.gallery_files.Any())
		{
			var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "images");
			if (!Directory.Exists(uploadPath))
			{
				Directory.CreateDirectory(uploadPath);
			}
			foreach(var file in entity.gallery)
			{
				if (System.IO.File.Exists(file))
				{
					System.IO.File.Delete(file);
				}
			}
			List<string> files = new();
			foreach (var file in src.gallery_files)
			{
				var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
				var imagePath = Path.Combine(uploadPath, fileName);

				// Save Image
				using (var stream = new FileStream(imagePath, FileMode.Create))
				{
					await file.CopyToAsync(stream);
				}
				files.Add(imagePath);
			}
			entity.gallery = files;
		}
		var code = "";
		do
		{
			code = RandomGenerator.GenerateString(10, AllowedCharacters.Alphanumeric);
		}
		while (await _unitOfWork.PropertyRepository.ExistsAsync(x => x.code == code));

		entity.updated_at = DateTime.UtcNow;
		entity.slug = slug;
		entity.rent_price = src.rent_price;
		entity.meterage = src.meterage;
		entity.code = code;
		entity.property_floor = src.property_floor;
		entity.description = src.description;
		entity.property_age = src.property_age;
		entity.address = src.address;
		entity.bed_room_count = src.bed_room_count;
		entity.category_id = src.category_id;
		entity.city_id = src.city_id;
		entity.city_province_full_name = city.name + $"({city.province.name})";
		entity.is_for_sale = src.is_for_sale;
		entity.mortgage_price = src.mortgage_price;
		entity.name = src.name;
		entity.state_enum = src.state_enum;
		entity.type_enum = src.type_enum;
		entity.situation_id = src.situation_id;
		entity.sell_price = src.sell_price;
		entity.is_active = false;

		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<PropertyDto>()
		{
			data = null,
			is_success = true,
			message = "ملک با موفقیت ایجاد شد",
			response_code = 201
		});
	}


	[HttpPost]
	[Route("delete")]
	public async Task<IActionResult> Delete([FromBody] long id)
	{
		var entity = await _unitOfWork.PropertyRepository.Get(id);
		if (entity == null)
			return NotFound(new ResponseDto<PropertyDto>()
			{
				data = null,
				message = "ملک با این ایدی پیدا نشد",
				is_success=false,
				response_code = 404
			});
		var user = await GetCurrentUser();
		if (user == null)
			return NotFound(new ResponseDto<UserDto>()
			{
				data = null,
				message = "کاربر پیدا نشد",
				is_success = false,
				response_code = 404
			});
		foreach (var file in entity.gallery)
		{
			if (System.IO.File.Exists(file))
			{
				System.IO.File.Delete(file);
			}
		}
		
		_unitOfWork.PropertyRepository.Remove(entity);
		await _unitOfWork.CommitAsync();

		user.property_count++;
		_unitOfWork.UserRepository.Update(user);
		await _unitOfWork.CommitAsync();

		return Ok(new ResponseDto<PropertyDto>()
		{
			data = null,
			message = "ملک با موفقیت حذف شد",
			is_success = true,
			response_code = 204
		});
	}
}
