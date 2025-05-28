using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealState.Models;

namespace RealState.Controllers.Public;
[Route("api/public/property-gallery")]
[ApiController]
public class PublicPropertyGalleryController(IUnitOfWork unitOfWork, JwtTokenService tokenService) : ControllerBase
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
	public async Task<IActionResult> UserPropertyGallery()
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

		var data = await _unitOfWork.PropertyGalleryRepository.FindList(x=>x.user_id == userId);
		return Ok(new ResponseDto<List<PropertyGallery>>()
		{
			data = data,
			is_success = true,
			message = "",
			response_code = 200
		});
	}

	[HttpPost]
	[Route("create")]
	public async Task<IActionResult> Create([FromForm] PropertyGalleryDto src)
	{
		try
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

			var slug = "";
			do
			{
				slug = SlugHelper.GenerateSlug(Guid.NewGuid().ToString());
			}
			while (await _unitOfWork.PropertyGalleryRepository.ExistsAsync(x => x.slug == slug));

			if (src.picture_file == null)
				return BadRequest(new ResponseDto<PropertyDto>()
				{
					data = null,
					message = "مقدار فایل عکس را پر کنید",
					is_success = false,
					response_code = 400
				});
			var fileName = Guid.NewGuid().ToString() + Path.GetExtension(src.picture_file.FileName);
			var imagePath = Path.Combine(uploadPath, fileName);

			// Save Image
			using (var stream = new FileStream(imagePath, FileMode.Create))
			{
				await src.picture_file.CopyToAsync(stream);
			}
			await _unitOfWork.PropertyGalleryRepository.AddAsync(new PropertyGallery()
			{
				created_at = DateTime.Now,
				slug = slug,
				updated_at = DateTime.Now,
				alt = src.alt,
				picture = imagePath,
				user_id = user.id,
			});
			await _unitOfWork.CommitAsync();

			return Ok(new ResponseDto<string>()
			{
				data = null,
				is_success = true,
				message = "تصویر با موفقیت ایجاد شد",
				response_code = 201
			});
		}
		catch (Exception ex)
		{
			return BadRequest(new ResponseDto<string>()
			{
				data = ex.Message,
				message = ex.Message,
				is_success = false,
				response_code = 500
			});
		}
	}
}
