using DataLayer;
using DataLayer.Assistant.Enums;
using DataLayer.EntityFramework.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaelState.Models;
using RealState.Models;

namespace RealState.Controllers.Public;
[Route("api/public/contact")]
[ApiController]
public class PublicContactController(IUnitOfWork unitOfWork) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	[HttpPost]
	[Route("create")]
	public async Task<IActionResult> Create([FromForm] ContactDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<ContactDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		var slug = src.slug ?? SlugHelper.GenerateSlug(src.full_name + src.mobile + src.message);
		if (await _unitOfWork.ContactRepository.ExistsAsync(x => x.slug == slug))
		{
			var error = "مقدار پیام تکراریست لطفا تغییر دهید.";
			return BadRequest(new ResponseDto<ContactDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}

		await _unitOfWork.ContactRepository.AddAsync(new Contact()
		{
			created_at = DateTime.Now,
			updated_at = DateTime.Now,
			slug = slug,
			full_name = src.full_name,
			message = src.message,
			mobile = src.mobile,
		});
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<ContactDto>()
		{
			data = null,
			is_success = true,
			message = "پیام با موفقیت ایجاد شد.",
			response_code = 201
		});
	}

}