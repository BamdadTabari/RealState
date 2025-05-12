using DataLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaelState.Assistant;
using RaelState.Models;
using RealState;
using RealState.Models;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RaelState.Controllers;
[Route("api/login")]
[ApiController]
public class LoginController(IUnitOfWork unitOfWork, JwtTokenService jwtTokenService) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public static readonly SecurityTokenConfig Config = new();
    private readonly JwtTokenService _tokenService = jwtTokenService;

    [HttpPost]
    [Route("username")]
    public async Task<IActionResult> Login([FromForm] AdminLoginDto src)
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

        var user = await _unitOfWork.UserRepository.GetUser(src.user_name_or_email);
        if (user == null || !user.is_active)
		    return BadRequest(new ResponseDto<UserDto>()
		    {
			    data = null,
			    is_success = false,
			    message = "کاربر پیدا نشد یا تایید نشده.",
			    response_code = 400
		    });

		// بررسی قفل شدن کاربر
		if (user.is_locked_out && user.lock_out_end_time > DateTime.UtcNow)
		    return Unauthorized(new ResponseDto<UserDto>()
		    {
			    data = null,
			    is_success = false,
			    message = "اکانت شما موقتاً قفل شده است.",
			    response_code = 401
		    });
		// بررسی رمز عبور
		if (!PasswordHasher.Check(user.password_hash, src.password))
        {
            user.failed_login_count++;
            if (user.failed_login_count >= 5)
            {
                user.is_locked_out = true;
                user.lock_out_end_time = DateTime.UtcNow.AddMinutes(1); // قفل موقت
            }

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.CommitAsync();

			return Unauthorized(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = "رمز عبور اشتباه است.",
				response_code = 401
			});
		}

        // موفقیت در ورود
        user.failed_login_count = 0;
        user.last_login_date_time = DateTime.UtcNow;
        var role = _unitOfWork.UserRoleRepository.GetUserRolesByUserId(user.id);
        var token = "";
        do
        {
            token = _tokenService.GenerateToken(user, role.Select(x => x.role.title).ToList());
        }
        while (await _unitOfWork.TokenBlacklistRepository.ExistsAsync(x => x.token == token));
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.refresh_token = refreshToken;
        user.refresh_token_expiry_time = DateTime.UtcNow.Add(Config.AdminRefreshTokenLifetime);

        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.CommitAsync();

        // ذخیره توکن در کوکی
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,  // فقط از طریق جاوا اسکریپت دسترسی نداشته باشد
            Secure = true,    // فقط در HTTPS ارسال شود
            SameSite = SameSiteMode.None,  // تنظیمات سیاست کوکی
            Expires = DateTime.UtcNow.AddMinutes(Config.AccessTokenLifetime.TotalMinutes)  // زمان انقضا توکن
        };

        Response.Cookies.Append("jwt", token, cookieOptions);

        return Ok(new ResponseDto<LoginResponseDto>()
        {
            data = new LoginResponseDto()
            {
                access_token = token,
                refresh_token = refreshToken,
                expire_in = Config.AccessTokenLifetime.TotalMinutes
            },
            is_success = true,
            message = "ورود با موفقیت انجام شد",
            response_code = 200
        });
    }


    [HttpPost]
    [Route("logout")]
	[Authorize(Roles = "Admin,MainAdmin")]
	public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
			return NotFound(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = "کاربر پیدا نشد",
				response_code = 404
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
		var token = Request.Cookies["jwt"];
        if (string.IsNullOrWhiteSpace(token))
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = "داشتن توکن الزامی است",
				response_code = 400
			});

		var expiryMinutes = _tokenService.GetTokenExpiryMinutes(token);
        await _unitOfWork.TokenBlacklistRepository.AddAsync(new BlacklistedToken
        {
            token = token,
            expiry_date = DateTime.UtcNow.AddMinutes(expiryMinutes),
            slug = SlugHelper.GenerateSlug(token)
        });

        // حذف کوکی از مرورگر
        Response.Cookies.Delete("jwt");
        user.refresh_token = null;
        user.refresh_token_expiry_time = DateTime.MinValue;

        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.CommitAsync();

        return Ok(new ResponseDto<UserDto>()
        {
            data = null,
            is_success = true,
            response_code = 204,
            message = "با موفقیت خارج شدید."
		});
    }

    [HttpPost]
    [Route("refresh-token")]
	[Authorize(Roles = "Admin,MainAdmin")]
	public async Task<IActionResult> Refresh([FromForm] TokenRequestDto src)
    {
        // 1. اعتبارسنجی توکن
        if (!JwtHelper.Validate(src.token))
		    return BadRequest(new ResponseDto<UserDto>()
		    {
			    data = null,
			    is_success = false,
			    message = "Invalid token",
			    response_code = 400
		    });

		// 2. دریافت اطلاعات کاربر از توکن
		var username = JwtHelper.GetUsername(src.token);
        var user = await _unitOfWork.UserRepository.Get(username);

        // 3. بررسی صحت توکن و تاریخ انقضای Refresh Token
        if (user == null || user.refresh_token != src.refresh_token || user.refresh_token_expiry_time < DateTime.UtcNow)
		    return Unauthorized(new ResponseDto<UserDto>()
		    {
			    data = null,
			    is_success = false,
			    message = "Invalid refresh token or expired.",
			    response_code = 400
		    });

		// 4. تولید توکن جدید
		var role = _unitOfWork.UserRoleRepository.GetUserRolesByUserId(user.id);
		var token = "";
		do
		{
			token = _tokenService.GenerateToken(user, role.Select(x => x.role.title).ToList());
		}
		while (await _unitOfWork.TokenBlacklistRepository.ExistsAsync(x => x.token == token));
		var refreshToken = _tokenService.GenerateRefreshToken();

        // 5. به روزرسانی Refresh Token در پایگاه داده
        user.refresh_token = refreshToken;
        user.refresh_token_expiry_time = DateTime.UtcNow.Add(Config.AdminRefreshTokenLifetime);
        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.CommitAsync();

		// ذخیره توکن در کوکی
		var cookieOptions = new CookieOptions
		{
			HttpOnly = true,  // فقط از طریق جاوا اسکریپت دسترسی نداشته باشد
			Secure = true,    // فقط در HTTPS ارسال شود
			SameSite = SameSiteMode.None,  // تنظیمات سیاست کوکی
			Expires = DateTime.UtcNow.AddMinutes(Config.AccessTokenLifetime.TotalMinutes)  // زمان انقضا توکن
		};
		Response.Cookies.Append("jwt", token, cookieOptions);
		// 6. بازگشت توکن‌ها به کاربر
		return Ok(new ResponseDto<LoginResponseDto>()
        {
            data = new LoginResponseDto()
            {
				access_token = token,
				refresh_token = refreshToken,
                expire_in = Config.AccessTokenLifetime.TotalMinutes,
			},
            is_success = true,
            message = "",
            response_code = 200
        });
    }

    [HttpGet]
    [Route("profile")]
    [Authorize(Roles = "Admin,MainAdmin")]
    public async Task<IActionResult> GetUserProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
		    return NotFound(new ResponseDto<UserDto>()
		    {
			    data = null,
			    is_success = false,
			    message = "کاربر پیدا نشد",
			    response_code = 404
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
		return Ok(new ResponseDto<User>()
        {
            data = user,
            is_success = true,
            message = "",
            response_code = 200
		});
    }
}
