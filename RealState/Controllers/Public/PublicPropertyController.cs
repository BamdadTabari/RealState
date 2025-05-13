using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaelState.Models;
using RealState.Models;

namespace RealState.Controllers.Public;
[Route("api/public/property")]
[ApiController]
public class PublicPropertyController(IUnitOfWork unitOfWork, JwtTokenService tokenService) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly JwtTokenService _tokenService = tokenService;

	public async Task<long?> GetCurrentUserId()
	{
		var userId = _tokenService.GetUserIdFromClaims(User) ?? "0";
		var user = await _unitOfWork.UserRepository.GetUser(int.Parse(userId));
		return user?.id;
	}
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
			expire_date = plan.,
		});

		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<BlogDto>()
		{
			data = null,
			is_success = true,
			message = "بلاگ با موفقیت ایجاد شد",
			response_code = 201
		});
	}
}
