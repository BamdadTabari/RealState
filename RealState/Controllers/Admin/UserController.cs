using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaelState.Models;
using RealState.Models;

namespace RaelState.Controllers.Admin;
[Route("api/user")]
[ApiController]
[Authorize(Roles = "Admin,MainAdmin")]
public class UserController(IUnitOfWork unitOfWork) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	public static readonly SecurityTokenConfig Config = new();

	[HttpGet]
	[Route("list")]
	public IActionResult Index(string? search_term, bool? is_active, 
		SortByEnum sort_by = SortByEnum.CreationDate, int page = 1, int page_size = 10)
	{
		// Set up filter object
		var filter = new DefaultPaginationFilter(page, page_size)
		{
			Keyword = search_term,
			SortBy = sort_by,
			BoolFilter = is_active
		};
		var data = _unitOfWork.UserRepository.GetPaginated(filter);
		_unitOfWork.Dispose();
		
		return Ok(new ResponseDto<PaginatedList<User>>()
		{
			data = data,
			is_success = true,
			message = "",
			response_code = 200
		});
	}

	[HttpPost]
	[Route("create")]
	public async Task<IActionResult> Create([FromForm] AdminUserForRegistrationCommand src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				   .SelectMany(v => v.Errors)
				   .Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<BlogCategoryDto>()
			{
				data = new BlogCategoryDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		if (await _unitOfWork.UserRepository.ExistsAsync(x => x.user_name == src.user_name))
		{
			var error = "کاربر با این نام وجود دارد";
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = new UserDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.user_name);
		if (await _unitOfWork.UserRepository.ExistsAsync(x => x.slug == slug))
		{
			var error = "کاربر با این نامک وجود دارد";
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = new UserDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
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
		return Ok(new ResponseDto<UserDto>()
		{
			data = new UserDto(),
			is_success = true,
			message = "کاربر ایجاد شد.",
			response_code = 201
		});
	}

	[HttpPost]
	[Route("edit")]
	public async Task<IActionResult> Edit([FromForm]UserDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
			   .SelectMany(v => v.Errors)
			   .Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<BlogCategoryDto>()
			{
				data = new BlogCategoryDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var entity = await _unitOfWork.UserRepository.GetUser(src.id);
		if (entity == null)
			return NotFound(new ResponseDto<UserDto>()
			{
				data = new UserDto(),
				is_success = false,
				message = "کاربر با این ایدی وجود ندارد",
				response_code = 404
			});
		if (await _unitOfWork.UserRepository.ExistsAsync(x => x.user_name == src.user_name) && entity.user_name != src.user_name)
		{
			var error = "کاربر با این نام وجود دارد";
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = new UserDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.user_name);
		if (await _unitOfWork.UserRepository.ExistsAsync(x => x.slug == slug) && entity.slug != slug)
		{
			var error = "کاربر با این نامک وجود دارد";
			return BadRequest(new ResponseDto<UserDto>()
			{
				data = new UserDto(),
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		entity.slug = slug;
		entity.user_name = src.user_name;
		entity.password_hash = PasswordHasher.Hash(src.password);
		entity.updated_at = DateTime.Now;
		entity.email = src.email;
		entity.is_active = src.is_active;
		_unitOfWork.UserRepository.Update(entity);
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<UserDto>()
		{
			data = new UserDto(),
			is_success = true,
			message = "کاربر ویرایش شد",
			response_code = 204
		});
	}

	[HttpPost]
	[Route("delete")]
	public async Task<IActionResult> Delete([FromQuery]int id)
	{
		var entity = await _unitOfWork.UserRepository.GetUser(id);
		if (entity == null)
			return NotFound(new ResponseDto<UserDto>()
			{
				data = new UserDto(),
				is_success = true,
				message = "کاربر با این ایدی وجود ندارد",
				response_code = 404
			});
		_unitOfWork.UserRepository.Remove(entity);
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<UserDto>()
		{
			data = new UserDto(),
			is_success = true,
			message = "عملیات انجام شد",
			response_code = 204
		});
	}

	[HttpGet]
	[Route("read")]
	public async Task<IActionResult> Detail(string slug)
	{
		var entity = await _unitOfWork.UserRepository.Get(slug);
		if (entity == null)
			return NotFound(new ResponseDto<UserDto>()
			{
				data = new UserDto(),
				is_success = true,
				message = "کاربر با این slug وجود ندارد",
				response_code = 404
			});
		return Ok(new ResponseDto<UserDto>()
		{
			data = new UserDto()
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
			},
			is_success = true,
			message = "",
			response_code = 200
		});
	}

	[HttpPost]
	[Route("activate")]
	public async Task<IActionResult> CheckActive(int id)
	{
		var user = await _unitOfWork.UserRepository.GetUser(id);
		if (user == null)
			return NotFound(new ResponseDto<UserDto>()
			{
				data = new UserDto(),
				is_success = false,
				message = "کاربر با این ایدی وجود ندارد",
				response_code = 404
			});
		user.is_active = !user.is_active;
		_unitOfWork.UserRepository.Update(user);
		await _unitOfWork.CommitAsync();
		_unitOfWork.Dispose();
		return Ok(new ResponseDto<UserDto>()
		{
			data = new UserDto(),
			is_success = true,
			message = "وضعیت کاربر تغییر کرد",
			response_code = 200
		});
	}
}
