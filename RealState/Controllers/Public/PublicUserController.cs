using DataLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaelState.Assistant;
using RaelState.Models;
using RealState.Models;
using System.Security.Claims;
using TicketApi.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RealState.Controllers.Public;
[Route("api/public/user")]
[ApiController]
public class PublicUserController(JwtTokenService tokenService, IUnitOfWork unitOfWork) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	public static readonly SecurityTokenConfig Config = new();
	private readonly JwtTokenService _tokenService = tokenService;

	[HttpPost]
	[Route("user-login")]
	public async Task<IActionResult> Login([FromForm] LoginDto dto)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}

		var user = await _unitOfWork.UserRepository.Get(dto.phone_number);
		if (user == null)
			return NotFound(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = "کاربر پیدا نشد",
				response_code = 404
			});


		if (user == null || user.is_active == false)
		{
			var error = "کاربر پیدا نشد یا توسط ادمین غیر فعال شده";
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}

		// تولید کد OTP
		var otpCode = new Random().Next(1000, 9999);
		await _unitOfWork.OtpRepository.AddAsync(new Otp
		{
			phone = dto.phone_number,
			otp_code = otpCode,
			slug = SlugHelper.GenerateSlug(dto.phone_number + StampGenerator.CreateSecurityStamp(10))
		});
		await _unitOfWork.CommitAsync();

		// ارسال OTP
		SMSClass.SendOtp(dto.phone_number, otpCode.ToString());

		return Ok(new ResponseDto<UserDto>()
		{
			data = null,
			is_success = true,
			message = $"پبامک ورود به شماره {dto.phone_number} فرستاده شد.",
			response_code = 204
		});
	}

	[AllowAnonymous]
	[HttpPost]
	[Route("verify-phone")]
	public async Task<IActionResult> VerifyPhone([FromForm]VerifyPhoneDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var otp = await _unitOfWork.OtpRepository.GetByPhone(src.phone_number);
		if (otp == null)
		{
			var error = "کد وارد شده منقضی شده است. لطفاً دوباره تلاش کنید";
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}

		if (otp.otp_code != src.confirm_code)
		{
			var error = "مقدار نامعتبر است";
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}

		// تأیید کاربر
		var user = await _unitOfWork.UserRepository.GetUserByPhone(src.phone_number);

		if (user == null)
			return NotFound(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = true,
				message = "کاربر با این شماره تلفن وجود ندارد.",
				response_code = 401
			});
		var role = _unitOfWork.UserRoleRepository.GetUserRolesByUserId(user.id);
		var token = "";
		do
		{
			token = _tokenService.GenerateToken(user, role.Select(x => x.role.title).ToList());
		}
		while (await _unitOfWork.TokenBlacklistRepository.ExistsAsync(x => x.token == token));
		var refreshToken = "";
		do
		{
			refreshToken = _tokenService.GenerateRefreshToken();
		}
		while (await _unitOfWork.UserRepository.ExistsAsync(x => x.refresh_token == refreshToken));

		// تنظیم توکن در کوکی
		// ذخیره توکن در کوکی
		var cookieOptions = new CookieOptions
		{
			HttpOnly = true,  // فقط از طریق جاوا اسکریپت دسترسی نداشته باشد
			Secure = true,    // فقط در HTTPS ارسال شود
			SameSite = SameSiteMode.None,  // تنظیمات سیاست کوکی
			Expires = DateTime.UtcNow.AddMinutes(Config.AccessTokenLifetime.TotalMinutes)  // زمان انقضا توکن
		};
		Response.Cookies.Append("jwt", refreshToken, cookieOptions);

		// حذف تمام OTPهای مرتبط
		var allPhoneOtps = await _unitOfWork.OtpRepository.GetAllByPhone(src.phone_number);
		_unitOfWork.OtpRepository.RemoveRange(allPhoneOtps);
		await _unitOfWork.CommitAsync();
		// ذخیره RefreshToken در دیتابیس برای بررسی بعدی
		user.refresh_token = refreshToken;
		user.refresh_token_expiry_time = DateTime.UtcNow.Add(Config.RefreshTokenLifetime);
		user.is_mobile_confirmed = true;
		user.expire_date = DateTime.UtcNow.AddMonths(1);
		user.property_count = 5;
		_unitOfWork.UserRepository.Update(user);
		await _unitOfWork.CommitAsync();

		return Ok(new ResponseDto<LoginResponseDto>()
		{
			data = new LoginResponseDto()
			{
				access_token = token,
				//refresh_token = refreshToken,
				expire_in = Config.AccessTokenLifetime.TotalMinutes,
			},
			is_success = true,
			message = "عملیات موفقیت آمیز بود",
			response_code = 200
		});
	}

	[HttpPost]
	[Route("register")]
	public async Task<IActionResult> Register([FromForm] UserForRegistrationCommand request)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				   .SelectMany(v => v.Errors)
				   .Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		if (await _unitOfWork.UserRepository.ExistsAsync(x => x.mobile == request.phone_number))
		{
			var error = " شماره تلفن موجود است لطفا وارد شوید";
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		if (await _unitOfWork.UserRepository.ExistsAsync(x => x.email == request.email))
		{
			var error = "ایمیل موجود است";
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		if (await _unitOfWork.UserRepository.AnyExistUserName(request.user_name))
		{
			var error = "نام کاربری موجود است";
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}

		var slug = request.Agency.slug ?? SlugHelper.GenerateSlug(request.user_name);
		if (await _unitOfWork.UserRepository.ExistsAsync(x => x.slug == slug))
		{
			var error = "کاربر با این نامک وجود دارد";
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var agencySlug = request.Agency.slug ?? SlugHelper.GenerateSlug(request.Agency.full_name + Guid.NewGuid().ToString());
		if (await _unitOfWork.AgencyRepository.ExistsAsync(x => x.slug == agencySlug))
		{
			var error = "آژانس املاک با این نامک وجود دارد";
			return BadRequest(new ResponseDto<AgencyDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		// ایجاد کاربر
		User user = new()
		{
			updated_at = DateTime.Now,
			created_at = DateTime.Now,
			user_name = request.user_name,
			email = request.email,
			concurrency_stamp = StampGenerator.CreateSecurityStamp(32),
			security_stamp = StampGenerator.CreateSecurityStamp(32),
			failed_login_count = 0,
			is_active = true,
			is_locked_out = false,
			is_mobile_confirmed = false,
			last_login_date_time = DateTime.Now,
			mobile = request.phone_number,
			slug = slug
		};
		await _unitOfWork.UserRepository.AddAsync(user);
		await _unitOfWork.CommitAsync();
		// set role
		var role = await _unitOfWork.RoleRepository.GetRole("Customer");
		await _unitOfWork.UserRoleRepository.AddAsync(new UserRole()
		{
			created_at = DateTime.Now,
			updated_at = DateTime.Now,
			role_id = role.id,
			slug = $"Customer_{slug}",
			user_id = user.id,
		});
		await _unitOfWork.CommitAsync();
		var city = await _unitOfWork.CityRepository.Get(request.Agency.city_id);
		if (city == null)
			return NotFound(new ResponseDto<CityDto>()
			{
				data = null,
				is_success = false,
				message = "شهر پیدا نشد",
				response_code = 404
			});
		await _unitOfWork.AgencyRepository.AddAsync(new Agency()
		{
			agency_name = request.Agency.agency_name ?? "",
			created_at = DateTime.UtcNow,
			updated_at = DateTime.UtcNow,
			city_id = request.Agency.city_id,
			city_province_full_name = city.name + $"({city.province.name})",
			full_name = request.Agency.full_name,
			mobile = request.Agency.mobile,
			slug = agencySlug,
		});
		await _unitOfWork.CommitAsync();

		// تولید کد OTP
		var otpCode = new Random().Next(1000, 9999);
		await _unitOfWork.OtpRepository.AddAsync(new Otp
		{
			phone = request.phone_number,
			otp_code = otpCode,
			slug = SlugHelper.GenerateSlug(request.phone_number + Guid.NewGuid().ToString())
		});
		await _unitOfWork.CommitAsync();

		// ارسال OTP
		SMSClass.SendOtp(request.phone_number, otpCode.ToString());

		return Ok(new ResponseDto<UserDto>()
		{
			data = null,
			message = $"پبامک ورود به شماره {request.phone_number} فرستاده شد.",
			is_success = true,
			response_code = 200
		});
	}


	[AllowAnonymous]
	[HttpPost]
	[Route("verify-phone-register")]
	public async Task<IActionResult> VerifyPhoneRegister([FromForm] VerifyPhoneDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(error);
		}
		var otp = await _unitOfWork.OtpRepository.GetByPhone(src.phone_number);
		if (otp == null)
		{
			var error = "کد وارد شده منقضی شده است. لطفاً دوباره تلاش کنید";
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}

		if (otp.otp_code != src.confirm_code)
		{
			var error = "مقدار نامعتبر است";
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}

		// get user by phone
		var user = await _unitOfWork.UserRepository.GetUserByPhone(src.phone_number);
		if (user == null)
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = "کاربر با این شماره تلفن وجود ندارد",
				response_code = 400
			});

		var role = _unitOfWork.UserRoleRepository.GetUserRolesByUserId(user.id);
		var token = "";
		do
		{
			token = _tokenService.GenerateToken(user, role.Select(x => x.role.title).ToList());
		}
		while (await _unitOfWork.TokenBlacklistRepository.ExistsAsync(x => x.token == token));
		var refreshToken = "";
		do
		{
			refreshToken = _tokenService.GenerateRefreshToken();
		}
		while (await _unitOfWork.UserRepository.ExistsAsync(x => x.refresh_token == refreshToken));

		// تنظیم توکن در کوکی
		Response.Cookies.Append("jwt", refreshToken, new CookieOptions
		{
			HttpOnly = true, // جلوگیری از دسترسی جاوااسکریپت به کوکی
			Secure = true,
			SameSite = SameSiteMode.Strict,
			Expires = DateTimeOffset.UtcNow.AddDays(10) // زمان انقضا
		});

		// حذف تمام OTPهای مرتبط
		var allPhoneOtps = await _unitOfWork.OtpRepository.GetAllByPhone(src.phone_number);
		_unitOfWork.OtpRepository.RemoveRange(allPhoneOtps);
		await _unitOfWork.CommitAsync();
		// ذخیره RefreshToken در دیتابیس برای بررسی بعدی
		user.refresh_token = refreshToken;
		user.refresh_token_expiry_time = DateTime.UtcNow.Add(Config.RefreshTokenLifetime);
		user.is_mobile_confirmed = true;
		_unitOfWork.UserRepository.Update(user);
		await _unitOfWork.CommitAsync();

		return Ok(new ResponseDto<LoginResponseDto>()
		{
			data = new LoginResponseDto()
			{
				access_token = token,
				//refresh_token = refreshToken,
				expire_in = Config.AccessTokenLifetime.TotalMinutes
			},
			is_success = true,
			message = "",
			response_code = 200
		});
	}

	[HttpPost]
	[Route("user-refresh-token")]
	public async Task<IActionResult> Refresh()
	{

		if (Request.Cookies.TryGetValue("jwt", out string refreshToken))
		{
			var user = await _unitOfWork.UserRepository.FindSingle(x => x.refresh_token == refreshToken);
			if (user == null || user.refresh_token_expiry_time < DateTime.UtcNow)
				return Unauthorized(new ResponseDto<UserDto>()
				{
					data = null,
					is_success = false,
					message = "توکن نامعتبر است و یا منقضی شده است",
					response_code = 401
				});

			var role = _unitOfWork.UserRoleRepository.GetUserRolesByUserId(user.id);
			var token = "";
			do
			{
				token = _tokenService.GenerateToken(user, role.Select(x => x.role.title).ToList());
			}
			while (await _unitOfWork.TokenBlacklistRepository.ExistsAsync(x => x.token == token));
			
			return Ok(new ResponseDto<LoginResponseDto>()
			{
				data = new LoginResponseDto()
				{
					access_token = token,
					expire_in = Config.AccessTokenLifetime.TotalMinutes
				},
				is_success = true,
				message = "",
				response_code = 200
			});
		}
		else
		{
			return Unauthorized(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = "توکن یافت نشد",
				response_code = 401
			});
		}
	}

	[Authorize]
	[HttpGet]
	[Route("user-profile")]
	public async Task<IActionResult> GetUserProfile()
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId == null)
			return Unauthorized(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = "کاربر پیدا نشد",
				response_code = 401
			});
		var user = await _unitOfWork.UserRepository.GetUser(int.Parse(userId));
		if (user == null)
			return NotFound(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = "کاربر پیدا نشد",
				response_code = 404
			});
		return Ok(new ResponseDto<UserDto>()
		{
			data = new UserDto()
			{
				id = user.id,
				created_at = user.created_at,
				updated_at = user.updated_at,
				slug = user.slug,
				email = user.email,
				expire_date = user.expire_date,
				is_active = user.is_active,
				mobile = user.mobile,
				user_name = user.user_name,
			},
			is_success = true,
			message = "",
			response_code = 200
		});
	}

	[Authorize]
	[HttpPost]
	[Route("user-logout")]
	public async Task<IActionResult> Logout()
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId == null)
			return Unauthorized(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = "کاربر پیدا نشد",
				response_code = 401
			}); ;
		var user = await _unitOfWork.UserRepository.GetUser(int.Parse(userId));

		if (user == null)
			return NotFound(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = "کاربر پیدا نشد",
				response_code = 404
			});

		// پاک کردن رفرش توکن و زمان انقضای آن
		user.refresh_token = null;
		user.refresh_token_expiry_time = DateTime.MinValue;

		// به‌روزرسانی در دیتابیس
		_unitOfWork.UserRepository.Update(user);
		await _unitOfWork.CommitAsync();
		// حذف کوکی از مرورگر
		Response.Cookies.Delete("jwt");
		return Ok(new ResponseDto<UserDto>()
		{
			data = null,
			is_success = true,
			message = "با موفقیت از سیستم خارج شدید.",
			response_code = 200
		});
	}

}
