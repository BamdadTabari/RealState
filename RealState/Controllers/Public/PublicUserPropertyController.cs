﻿using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaelState.Models;
using RealState.Models;

namespace RealState.Controllers.Public;
[Route("api/public/user-property")]
[ApiController]
[Authorize]
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
		var user = await _unitOfWork.UserRepository.GetUser(long.Parse(userId));
		return user?.id;
	}
	[HttpPost]
	[Route("current-user")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<User?> GetCurrentUser()
	{
		var userId = _tokenService.GetUserIdFromClaims(User) ?? "0";
		var user = await _unitOfWork.UserRepository.GetUser(long.Parse(userId));
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
		var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "images");
		if (!Directory.Exists(uploadPath))
		{
			Directory.CreateDirectory(uploadPath);
		}
		var user = await GetCurrentUser();
		if (user == null)
			return NotFound(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = "کاربر یافت نشد. لطفا ابتدا ثبت نام کنید",
				response_code = 404
			});
		if (user.is_licensed ==  false)
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = "ابتدا احراز هویت خود را تکمیل کنید",
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
		if (user.expire_date < DateTime.Now)
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				message = "پلن شما منقضی شده است. لطفا پلن خود را شارژ کنید",
				is_success = false,
				response_code = 400
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
		
		if (src.gallery == null || !src.gallery.Any())
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
		var filePath = "";
		if (src.video_file != null)
		{
			var allowedExtensions = new[] { ".mp4", ".mov", ".avi", ".wmv", ".flv", ".mkv", ".webm", ".mpeg" };
			var ext = Path.GetExtension(src.video_file.FileName).ToLower();
			if (!allowedExtensions.Contains(ext))
				return BadRequest(new ResponseDto<PropertyDto>()
				{
					data = null,
					is_success = false,
					message = "نوع فایل ویدیو نامعتبر است",
					response_code = 400
				});

			var fileName = Guid.NewGuid() + Path.GetExtension(src.video_file.FileName);
			filePath = Path.Combine(uploadPath, fileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await src.video_file.CopyToAsync(stream);
			}
		}
		var code = "";
		do
		{
			code = RandomGenerator.GenerateString(10, AllowedCharacters.Alphanumeric);
		}
		while(await _unitOfWork.PropertyRepository.ExistsAsync(x=>x.code == code));
		
		var property = await _unitOfWork.PropertyRepository.AddAsyncReturnid(new()
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
			//gallery = files,
			is_active = false,
			video = filePath,
			video_caption = src.video_caption ?? ""
		});

		await _unitOfWork.CommitAsync();

		List<PropertyGallery> files = new();
		foreach (var file in src.gallery)
		{
			if (file.picture_file == null)
				return BadRequest(new ResponseDto<PropertyDto>()
				{
					data = null,
					message = "مقدار فایل عکس را پر کنید",
					is_success = false,
					response_code = 400
				});
			var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.picture_file.FileName);
			var imagePath = Path.Combine(uploadPath, fileName);

			// Save Image
			using (var stream = new FileStream(imagePath, FileMode.Create))
			{
				await file.picture_file.CopyToAsync(stream);
			}
			files.Add(new PropertyGallery()
			{
				alt = file.alt,
				created_at = DateTime.Now,
				updated_at = DateTime.Now,
				picture = imagePath,
				property_id = property.id,
				slug = SlugHelper.GenerateSlug(file.alt + Guid.NewGuid().ToString()),
			});
		}
		await _unitOfWork.PropertyGalleryRepository.AddRangeAsync(files);
		await _unitOfWork.CommitAsync();

		user.property_count--;
		_unitOfWork.UserRepository.Update(user);
		await _unitOfWork.CommitAsync();

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
		if (user.is_licensed == false)
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = "ابتدا احراز هویت خود را تکمیل کنید",
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
		if (user.expire_date < DateTime.Now)
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				message = "پلن شما منقضی شده است. لطفا پلن خود را شارژ کنید",
				is_success = false,
				response_code = 400
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
		var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "images");
		if (!Directory.Exists(uploadPath))
		{
			Directory.CreateDirectory(uploadPath);
		}
		if (src.gallery != null && src.gallery.Any())
		{
			foreach (var file in entity.gallery)
			{
				if (System.IO.File.Exists(file.picture))
				{
					System.IO.File.Delete(file.picture);
				}
			}
			_unitOfWork.PropertyGalleryRepository.RemoveRange(entity.gallery);
			await _unitOfWork.CommitAsync();
			List<PropertyGallery> files = new();
			foreach (var file in src.gallery)
			{
				if (file.picture_file == null)
					return BadRequest(new ResponseDto<PropertyDto>()
					{
						data = null,
						message = "مقدار فایل عکس را پر کنید",
						is_success = false,
						response_code = 400
					});
				var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.picture_file.FileName);
				var imagePath = Path.Combine(uploadPath, fileName);

				// Save Image
				using (var stream = new FileStream(imagePath, FileMode.Create))
				{
					await file.picture_file.CopyToAsync(stream);
				}
				files.Add(new PropertyGallery()
				{
					alt = file.alt,
					created_at = DateTime.Now,
					updated_at = DateTime.Now,
					picture = imagePath,
					property_id = entity.id,
					slug = SlugHelper.GenerateSlug(file.alt + Guid.NewGuid().ToString()),
				});
			}
			await _unitOfWork.PropertyGalleryRepository.AddRangeAsync(files);
			await _unitOfWork.CommitAsync();
		}

		var filePath = "";
		if (src.video_file != null)
		{
			var allowedExtensions = new[] { ".mp4", ".mov", ".avi", ".wmv", ".flv", ".mkv", ".webm", ".mpeg" };
			var ext = Path.GetExtension(src.video_file.FileName).ToLower();
			if (!allowedExtensions.Contains(ext))
				return BadRequest(new ResponseDto<PropertyDto>()
				{
					data = null,
					is_success = false,
					message = "نوع فایل ویدیو نامعتبر است",
					response_code = 400
				});

			var fileName = Guid.NewGuid() + Path.GetExtension(src.video_file.FileName);
			filePath = Path.Combine(uploadPath, fileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await src.video_file.CopyToAsync(stream);
			}
		}
		var code = "";
		do
		{
			code = RandomGenerator.GenerateString(10, AllowedCharacters.Alphanumeric);
		}
		while (await _unitOfWork.PropertyRepository.ExistsAsync(x => x.code == code));

		entity.updated_at = DateTime.Now;
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
		entity.video = filePath;
		entity.video_caption = src.video_caption ?? "";
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
	public async Task<IActionResult> Delete([FromForm] long id)
	{
		var entity = await _unitOfWork.PropertyRepository.Get(id);
		if (entity == null)
			return NotFound(new ResponseDto<PropertyDto>()
			{
				data = null,
				message = "ملک با این ایدی پیدا نشد",
				is_success = false,
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
			if (System.IO.File.Exists(file.picture))
			{
				System.IO.File.Delete(file.picture);
			}
		}
		_unitOfWork.PropertyGalleryRepository.RemoveRange(entity.gallery);
		await _unitOfWork.CommitAsync();
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
