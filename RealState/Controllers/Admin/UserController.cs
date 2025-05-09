using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Mvc;
using RaelState.Models;
using RealState.Models;

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
	public async Task<IActionResult> Create(AdminUserForRegistrationCommand src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				   .SelectMany(v => v.Errors)
				   .Select(e => e.ErrorMessage));
			return BadRequest(error);
		}
		if (await _unitOfWork.UserRepository.ExistsAsync(x => x.user_name == src.user_name))
		{
			var error = "کاربر با این نام وجود دارد";
			return BadRequest(error);
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.user_name);
		if (await _unitOfWork.UserRepository.ExistsAsync(x => x.slug == slug))
		{
			var error = "کاربر با این نامک وجود دارد";
			return BadRequest(error);
		}
		User user = new()
		{
			updated_at = DateTime.Now,
			created_at = DateTime.Now,
			user_name = src.user_name,
			email = src.email,
			concurrency_stamp = StampGenerator.CreateSecurityStamp(32),
			security_stamp = StampGenerator.CreateSecurityStamp(32),
			failed_login_count = 0,
			is_active = src.is_active,
			is_locked_out = false,
			is_mobile_confirmed = true,
			last_login_date_time = DateTime.Now,
			password_hash = PasswordHasher.Hash(src.password),
			mobile = src.phone_number,
			slug = slug
		};
		await _unitOfWork.UserRepository.AddAsync(user);
		await _unitOfWork.CommitAsync();
		var role = await _unitOfWork.RoleRepository.GetRole("Admin");
		await _unitOfWork.UserRoleRepository.AddAsync(new UserRole()
		{
			created_at = DateTime.Now,
			updated_at = DateTime.Now,
			role_id = role.id,
			slug = $"Admin_{slug}",
			user_id = user.id,
		});
		await _unitOfWork.CommitAsync();
		return Ok();
	}

	[HttpPost]
	[Route("update-user")]
	public async Task<IActionResult> Edit(UserDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
			   .SelectMany(v => v.Errors)
			   .Select(e => e.ErrorMessage));
			return BadRequest(error);
		}
		var entity = await _unitOfWork.UserRepository.GetUser(src.id);
		if (entity == null)
			return NotFound("کاربر با این ایدی وجود ندارد");
		if (await _unitOfWork.UserRepository.ExistsAsync(x => x.user_name == src.user_name) && entity.user_name != src.user_name)
		{
			var error = "کاربر با این نام وجود دارد";
			return BadRequest(error);
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.user_name);
		if (await _unitOfWork.UserRepository.ExistsAsync(x => x.slug == slug) && entity.slug != slug)
		{
			var error = "کاربر با این نامک وجود دارد";
			return BadRequest(error);
		}
		entity.slug = slug;
		entity.user_name = src.user_name;
		entity.password_hash = PasswordHasher.Hash(src.password);
		entity.updated_at = DateTime.Now;
		entity.email = src.email;
		entity.is_active = src.is_active;
		_unitOfWork.UserRepository.Update(entity);
		await _unitOfWork.CommitAsync();
		return Ok();
	}

	[HttpPost]
	[Route("delete-user/{id}")]
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
	public async Task<IActionResult> Detail(string slug)
	{
		var entity = await _unitOfWork.UserRepository.Get(slug);
		if (entity == null)
			return NotFound("کاربر با این slug وجود ندارد");
		return Ok(new UserDto()
		{
			id = entity.id,
			created_at = entity.created_at,
			updated_at = entity.updated_at,
			slug = slug,
			mobile = entity.mobile,
			is_mobile_confirmed = entity.is_mobile_confirmed,
			is_active = entity.is_active,
			failed_login_count = entity.failed_login_count,
			email = entity.email,
			last_login_date_time = entity.last_login_date_time,
			user_name = entity.user_name,
			user_roles = entity.user_roles == null ? [] :
			entity.user_roles.Select(y => new UserRoleDto()
			{
				id = y.id,
				created_at = y.created_at,
				updated_at = y.updated_at,
				slug = y.slug,
				role_id = y.role_id,
				user_id = y.user_id
			}).ToList(),
		});
	}

	[HttpPost]
	[Route("activate-and-deactive-user")]
	public async Task<IActionResult> CheckActive(int id)
	{
		var user = await _unitOfWork.UserRepository.GetUser(id);
		if (user == null)
			return NotFound("کاربر با این ایدی وجود ندارد");
		user.is_active = !user.is_active;
		_unitOfWork.UserRepository.Update(user);
		await _unitOfWork.CommitAsync();
		_unitOfWork.Dispose();
		return RedirectToAction("Detail", "User", new { Area = "Admin", slug = user.slug });
	}
}
