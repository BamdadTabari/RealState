using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Mvc;
using RaelState.Assistant;
using RaelState.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RaelState.Controllers.Admin;
[Route("api/user")]
[ApiController]
public class UserController(IUnitOfWork unitOfWork, JwtTokenService jwtTokenService) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	public static readonly SecurityTokenConfig Config = new();
	private readonly JwtTokenService _tokenService = jwtTokenService;

	[HttpGet]
	[Route("home")]
	[HasPermission("User.Index")]
	public IActionResult Index(string searchTerm, bool? boolFilter, SortByEnum sortBy = SortByEnum.CreationDate, int page = 1, int pageSize = 10)
	{
		// Set up filter object
		var filter = new DefaultPaginationFilter(page, pageSize)
		{
			Keyword = searchTerm,
			SortBy = sortBy,
			BoolFilter = boolFilter
		};
		var data = _unitOfWork.UserRepository.GetPaginated(filter);
		_unitOfWork.Dispose();
		return Ok(data);
	}

	[HttpPost]
	[Route("create-user")]
	[HasPermission("User.Create")]
	public async Task<IActionResult> Create(AdminUserForRegistrationCommand src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				   .SelectMany(v => v.Errors)
				   .Select(e => e.ErrorMessage));
			return BadRequest(error);
		}
		if (await _unitOfWork.UserRepository.ExistsAsync(x => x.Username == src.UserName))
		{
			var error = "کاربر با این نام وجود دارد";
			return BadRequest(error);
		}
		var slug = src.Slug ?? SlugHelper.GenerateSlug(src.UserName);
		if (await _unitOfWork.UserRepository.ExistsAsync(x => x.Slug == slug))
		{
			var error = "کاربر با این نامک وجود دارد";
			return BadRequest(error);
		}
		User user = new()
		{
			UpdatedAt = DateTime.Now,
			CreatedAt = DateTime.Now,
			Username = src.UserName,
			Email = src.Email,
			ConcurrencyStamp = StampGenerator.CreateSecurityStamp(32),
			SecurityStamp = StampGenerator.CreateSecurityStamp(32),
			FailedLoginCount = 0,
			IsActive = src.IsActive,
			IsLockedOut = false,
			IsMobileComfirmed = true,
			LastLoginDate = DateTime.Now,
			PasswordHash = PasswordHasher.Hash(src.Password),
			Mobile = src.PhoneNumber,
			Slug = slug
		};
		await _unitOfWork.UserRepository.AddAsync(user);
		await _unitOfWork.CommitAsync();
		var role = await _unitOfWork.RoleRepository.GetRole("Admin");
		await _unitOfWork.UserRoleRepository.AddAsync(new UserRole()
		{
			CreatedAt = DateTime.Now,
			UpdatedAt = DateTime.Now,
			RoleId = role.Id,
			Slug = $"Admin_{slug}",
			UserId = user.Id,
		});
		await _unitOfWork.CommitAsync();
		return Ok();
	}

	[HttpPost]
	[Route("update-user")]
	[HasPermission("User.Edit")]
	public async Task<IActionResult> Edit(UserDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
			   .SelectMany(v => v.Errors)
			   .Select(e => e.ErrorMessage));
			return BadRequest(error);
		}
		var entity = await _unitOfWork.UserRepository.GetUser(src.Id);
		if (entity == null)
			return NotFound("کاربر با این ایدی وجود ندارد");
		if (await _unitOfWork.UserRepository.ExistsAsync(x => x.Username == src.Username) && entity.Username != src.Username)
		{
			var error = "کاربر با این نام وجود دارد";
			return BadRequest(error);
		}
		var slug = src.Slug ?? SlugHelper.GenerateSlug(src.Username);
		if (await _unitOfWork.UserRepository.ExistsAsync(x => x.Slug == slug) && entity.Slug != slug)
		{
			var error = "کاربر با این نامک وجود دارد";
			return BadRequest(error);
		}
		entity.Slug = slug;
		entity.Username = src.Username;
		entity.PasswordHash = PasswordHasher.Hash(src.Password);
		entity.UpdatedAt = DateTime.Now;
		entity.Email = src.Email;
		entity.IsActive = src.IsActive;
		_unitOfWork.UserRepository.Update(entity);
		await _unitOfWork.CommitAsync();
		return Ok();
	}

	[HttpPost]
	[Route("delete-user/{id}")]
	[HasPermission("User.Delete")]
	public async Task<IActionResult> Delete([FromRoute]int id)
	{
		var entity = await _unitOfWork.UserRepository.GetUser(id);
		if (entity == null)
			return NotFound("کاربر با این ایدی وجود ندارد");
		_unitOfWork.UserRepository.Remove(entity);
		await _unitOfWork.CommitAsync();
		_unitOfWork.Dispose();
		return RedirectToAction("Index", "User", new { Area = "Admin" });
	}

	[HttpGet]
	[Route("detail-user")]
	[HasPermission("User.Detail")]
	public async Task<IActionResult> Detail(string slug)
	{
		var entity = await _unitOfWork.UserRepository.Get(slug);
		if (entity == null)
			return NotFound("کاربر با این slug وجود ندارد");
		return Ok(new UserDto()
		{
			Id = entity.Id,
			CreatedAt = entity.CreatedAt,
			UpdatedAt = entity.UpdatedAt,
			Slug = slug,
			Mobile = entity.Mobile,
			IsMobileConfirmed = entity.IsMobileComfirmed,
			IsActive = entity.IsActive,
			FailedLoginCount = entity.FailedLoginCount,
			Email = entity.Email,
			LastLoginDate = entity.LastLoginDate,
			Username = entity.Username,
			UserRoles = entity.UserRoles == null ? [] :
			entity.UserRoles.Select(y => new UserRoleDto()
			{
				Id = y.Id,
				CreatedAt = y.CreatedAt,
				UpdatedAt = y.UpdatedAt,
				Slug = y.Slug,
				RoleId = y.RoleId,
				UserId = y.UserId
			}).ToList(),
		});
	}

	[HttpPost]
	[Route("activate-and-deactive-user")]
	[HasPermission("User.CheckActive")]
	public async Task<IActionResult> CheckActive(int id)
	{
		var user = await _unitOfWork.UserRepository.GetUser(id);
		if (user == null)
			return NotFound("کاربر با این ایدی وجود ندارد");
		user.IsActive = !user.IsActive;
		_unitOfWork.UserRepository.Update(user);
		await _unitOfWork.CommitAsync();
		_unitOfWork.Dispose();
		return RedirectToAction("Detail", "User", new { Area = "Admin", slug = user.Slug });
	}
}
