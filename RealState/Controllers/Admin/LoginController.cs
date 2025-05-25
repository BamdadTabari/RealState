using DataLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
		if (user.is_locked_out && user.lock_out_end_time > DateTime.Now)
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
                user.lock_out_end_time = DateTime.Now.AddMinutes(1); // قفل موقت
            }

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.CommitAsync();

			return Unauthorized(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = false,
				message = "رمز عبور یا نام کاربری اشتباه است.",
				response_code = 401
			});
		}

        // موفقیت در ورود
        user.failed_login_count = 0;
        user.last_login_date_time = DateTime.Now;
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
		

        user.refresh_token = refreshToken;
        user.refresh_token_expiry_time = DateTime.Now.Add(Config.AdminRefreshTokenLifetime);

        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.CommitAsync();

		// ذخیره توکن در کوکی
		var cookieOptions = new CookieOptions
		{
			HttpOnly = true,
			Secure = false, // موقتا غیر فعال کن برای تست
			SameSite = SameSiteMode.Lax, // یا None اگر لازم بود
			Expires = DateTime.Now.AddMinutes(Config.AdminRefreshTokenLifetime.TotalMinutes)
		};

		Response.Cookies.Append("jwt", refreshToken, cookieOptions);

        return Ok(new ResponseDto<LoginResponseDto>()
        {
            data = new LoginResponseDto()
            {
                access_token = token,
                //refresh_token = refreshToken,
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
		// بررسی اینکه هدر Authorization وجود داره یا نه
		if (Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
		{
			// مقدار هدر معمولاً به شکل "Bearer {access_token}" هست
			var token = authorizationHeader.ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase).Trim();

			if (string.IsNullOrWhiteSpace(token))
				return BadRequest(new ResponseDto<UserDto>()
				{
					data = null,
					is_success = false,
					message = "داشتن توکن الزامی است",
					response_code = 400
				});

			await _unitOfWork.TokenBlacklistRepository.AddAsync(new BlacklistedToken
			{
				token = token,
				expiry_date = DateTime.Now.AddDays(Config.AccessTokenLifetime.TotalDays),
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
		else
		{
			return Unauthorized(new ResponseDto<UserDto>()
			{
				data = null,
				is_success = true,
				response_code = 401,
				message = "توکن در هدر وجود ندارد."
			});
		}
    }

    [HttpPost]
    [Route("refresh-token")]
	public async Task<IActionResult> Refresh()
    {
		 
		if (Request.Cookies.TryGetValue("jwt", out string refreshToken))
		{
			var user = await _unitOfWork.UserRepository.FindSingle(x=>x.refresh_token == refreshToken);
			if (user == null ||  user.refresh_token_expiry_time < DateTime.Now)
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
			//var newRefreshToken = "";
			//do
			//{
			//	newRefreshToken = _tokenService.GenerateRefreshToken();
			//}
			//while (await _unitOfWork.UserRepository.ExistsAsync(x => x.refresh_token == newRefreshToken));

			//user.refresh_token = newRefreshToken;
			//user.refresh_token_expiry_time = DateTime.Now.Add(Config.AdminRefreshTokenLifetime);

			//_unitOfWork.UserRepository.Update(user);
			//await _unitOfWork.CommitAsync();

			//var cookieOptions = new CookieOptions
			//{
			//	HttpOnly = true,
			//	Secure = false, // موقتا غیر فعال کن برای تست
			//	SameSite = SameSiteMode.Lax, // یا None اگر لازم بود
			//	Expires = DateTime.Now.AddMinutes(Config.AccessTokenLifetime.TotalMinutes)
			//};

			//Response.Cookies.Append("jwt", newRefreshToken, cookieOptions);
			return Ok(new ResponseDto<LoginResponseDto>()
			{
				data = new LoginResponseDto()
				{
					access_token = token,
					expire_in = Config.AccessTokenLifetime.TotalMinutes,
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
}
