using DataLayer;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using RaelState.Assistant;

namespace RaelState.Controllers;
[Route("api/login")]
[ApiController]
public class LoginController(IUnitOfWork unitOfWork, JwtTokenService jwtTokenService) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public static readonly SecurityTokenConfig Config = new();
    private readonly JwtTokenService _tokenService = jwtTokenService;

    [HttpGet("whoami")]
    public IActionResult WhoAmI()
    {
        return Ok(new
        {
            IsAuthenticated = User.Identity.IsAuthenticated,
            Claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }
    [HttpPost]
    [Route("admin-login")]
    public async Task<IActionResult> Login([FromForm] AdminLoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            var error = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            return BadRequest(error);
        }

        var user = await _unitOfWork.UserRepository.GetUser(loginDto.UserNameOrEmail);
        if (user == null || !user.IsActive)
            return BadRequest("کاربر پیدا نشد یا تایید نشده.");

        // بررسی قفل شدن کاربر
        if (user.IsLockedOut && user.LockoutEndTime > DateTime.UtcNow)
            return Unauthorized("اکانت شما موقتاً قفل شده است.");

        // بررسی رمز عبور
        if (!PasswordHasher.Check(user.PasswordHash, loginDto.Password))
        {
            user.FailedLoginCount++;
            if (user.FailedLoginCount >= 5)
            {
                user.IsLockedOut = true;
                user.LockoutEndTime = DateTime.UtcNow.AddMinutes(1); // قفل موقت
            }

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.CommitAsync();

            return Unauthorized("رمز عبور اشتباه است.");
        }

        // موفقیت در ورود
        user.FailedLoginCount = 0;
        user.LastLoginDate = DateTime.UtcNow;
        var role = _unitOfWork.UserRoleRepository.GetUserRolesByUserId(user.Id);
        var token = "";
        do
        {
            token = _tokenService.GenerateToken(user, role.Select(x => x.Role.Title).ToList());
        }
        while (await _unitOfWork.TokenBlacklistRepository.ExistsAsync(x => x.Token == token));
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.Add(Config.AdminRefreshTokenLifetime);

        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.CommitAsync();

        // ذخیره توکن در کوکی
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,  // فقط از طریق جاوا اسکریپت دسترسی نداشته باشد
            Secure = true,    // فقط در HTTPS ارسال شود
            SameSite = SameSiteMode.Lax,  // تنظیمات سیاست کوکی
            Expires = DateTime.UtcNow.AddMinutes(15)  // زمان انقضا توکن
        };

        Response.Cookies.Append("jwt", token, cookieOptions);

        return Ok(new
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpireIn = Config.AccessTokenLifetime.TotalMinutes
        });
    }


    [HttpPost]
    [Route("admin-logout")]
    [HasPermission("Login.Logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return BadRequest("کاربر پیدا نشد");
        var user = await _unitOfWork.UserRepository.GetUser(int.Parse(userId));
        if (user == null)
            return NotFound("کاربر پیدا نشد.");
        var token = Request.Cookies["jwt"];
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest("Token is required");

        var expiryMinutes = _tokenService.GetTokenExpiryMinutes(token);
        await _unitOfWork.TokenBlacklistRepository.AddAsync(new BlacklistedToken
        {
            Token = token,
            ExpiryDate = DateTime.UtcNow.AddMinutes(expiryMinutes),
            Slug = SlugHelper.GenerateSlug(token)
        });

        // حذف کوکی از مرورگر
        Response.Cookies.Delete("jwt");
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.MinValue;

        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.CommitAsync();

        return Ok("با موفقیت خارج شدید.");
    }

    [HttpPost]
    [Route("admin-refresh-token")]
    public async Task<IActionResult> Refresh([FromForm] TokenRequestDto request)
    {
        // 1. اعتبارسنجی توکن
        if (!JwtHelper.Validate(request.Token))
            return BadRequest("Invalid token");

        // 2. دریافت اطلاعات کاربر از توکن
        var username = JwtHelper.GetUsername(request.Token);
        var user = await _unitOfWork.UserRepository.Get(username);

        // 3. بررسی صحت توکن و تاریخ انقضای Refresh Token
        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            return Unauthorized("Invalid refresh token or expired.");

        // 4. تولید توکن جدید
        var role = _unitOfWork.UserRoleRepository.GetUserRolesByUserId(user.Id);
		var token = "";
		do
		{
			token = _tokenService.GenerateToken(user, role.Select(x => x.Role.Title).ToList());
		}
		while (await _unitOfWork.TokenBlacklistRepository.ExistsAsync(x => x.Token == token));
		var refreshToken = _tokenService.GenerateRefreshToken();

        // 5. به روزرسانی Refresh Token در پایگاه داده
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.Add(Config.AdminRefreshTokenLifetime);
        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.CommitAsync();

        // 6. بازگشت توکن‌ها به کاربر
        return Ok(new
        {
            Token = token,
            RefreshToken = refreshToken,
        });
    }

    [HttpGet]
    [Route("admin-profile")]
    [HasPermission("Login.GetUserProfile")]
    public async Task<IActionResult> GetUserProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return BadRequest("کاربر پیدا نشد");
        var user = await _unitOfWork.UserRepository.GetUser(int.Parse(userId));
        return Ok(user);
    }
}
