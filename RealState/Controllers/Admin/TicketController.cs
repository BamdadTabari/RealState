using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealState.Models;

namespace RealState.Controllers.Admin;
[Route("api/ticket")]
[ApiController]
[Authorize(Roles = "Admin,MainAdmin")]
public class TicketController(IUnitOfWork unitOfWork) : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	[HttpGet]
	[Route("list")]
	public async Task<IActionResult> List(string? search_term, TicketStatus? ticketStatus, int page = 1, int page_size = 10)
	{
		var filter = new DefaultPaginationFilter(page, page_size)
		{
			Keyword = search_term,
			SortBy = SortByEnum.CreationDate,
			TicketStatus = ticketStatus,
		};

		var data = _unitOfWork.TicketRepository.GetPaginated(filter, null);
		return Ok(new ResponseDto<PaginatedList<Ticket>>()
		{
			data = data,
			is_success = true,
			message = "",
			response_code = 200
		});
	}

	[HttpGet]
	[Route("read/{slug}")]
	public async Task<IActionResult> Read(string slug)
	{
		var ticket = await _unitOfWork.TicketRepository.Get(slug);
		if (ticket == null)
		{
			return NotFound(new ResponseDto<Ticket>()
			{
				data = null,
				is_success = false,
				message = "تیکت یافت نشد",
				response_code = 404
			});
		}

		return Ok(new ResponseDto<TicketDto>()
		{
			data = new TicketDto()
			{
				id = ticket.id,
				created_at = ticket.created_at,
				updated_at = ticket.updated_at,
				slug = slug,
				is_admin = ticket.is_admin,
				message = ticket.message,
				picture = ticket.picture,
				status = ticket.status,
				ticket_code = ticket.ticket_code,
				subject = ticket.subject,
				user_id = ticket.user_id,
				replies = ticket.replies == null ? new List<TicketReplyDto>() :
				ticket.replies.Select(x => new TicketReplyDto()
				{
					id = x.id,
					created_at = x.created_at,
					updated_at = x.updated_at,
					slug = x.slug,
					is_admin = x.is_admin,
					message = x.message,
					picture = x.picture,
					ticket_id = x.ticket_id,
					user_id = x.user_id,
				}).ToList()
			},
			is_success = true,
			message = "",
			response_code = 200
		});
	}
	[HttpPost]
	[Route("create")]
	public async Task<IActionResult> Create([FromForm] TicketDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<TicketDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
		if(src.user_id == 0)
			return BadRequest(new ResponseDto<TicketDto>()
			{
				data = null,
				message = "آیدی کاربر را وارد کنید",
				is_success = false,
				response_code = 400
			});
		var user = await _unitOfWork.UserRepository.GetUser(src.user_id);
		if (user == null)
			return NotFound(new ResponseDto<TicketDto>()
			{
				data = null,
				message = "کاربر یافت نشد",
				is_success = false,
				response_code = 404
			});
		var code = "";
		do
		{
			code = RandomGenerator.GenerateCode(12);
		} while (await _unitOfWork.TicketRepository.ExistsAsync(x => x.ticket_code == code));
		var ticket = new Ticket()
		{
			message = src.message,
			user_id = src.user_id,
			created_at = DateTime.Now,
			updated_at = DateTime.Now,
			slug = SlugHelper.GenerateSlug(code),
			is_admin = true,
			status = TicketStatus.Open,
			subject = src.subject,
			ticket_code = code,
		};

		if (src.picture_file != null)
		{
			// Define the directory for uploads 
			var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "images");

			// Create directory if not Exist
			if (!Directory.Exists(uploadPath))
			{
				Directory.CreateDirectory(uploadPath);
			}

			// Build file name
			var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(src.picture_file.FileName);
			var imagePath = Path.Combine(uploadPath, newFileName);

			// Save Image
			using (var stream = new FileStream(imagePath, FileMode.Create))
			{
				await src.picture_file.CopyToAsync(stream);
			}
			ticket.picture = imagePath;
		}

		await _unitOfWork.TicketRepository.AddAsync(ticket);

		await _unitOfWork.CommitAsync();

		return Ok(new ResponseDto<TicketDto>()
		{
			data = null,
			is_success = true,
			message = "تیکت جدید ثبت شد",
			response_code = 201
		});
	}

	[HttpPost]
	[Route("reply")]
	public async Task<IActionResult> Reply([FromForm] TicketReplyDto src)
	{
		if (!ModelState.IsValid)
		{
			var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<TicketReply>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}

		var ticket = await _unitOfWork.TicketRepository.Get(src.ticket_id);
		if (ticket == null)
		{
			return NotFound(new ResponseDto<TicketReply>()
			{
				data = null,
				is_success = false,
				message = "تیکت مورد نظر یافت نشد",
				response_code = 404
			});
		}
		if (src.user_id == 0)
			return BadRequest(new ResponseDto<TicketDto>()
			{
				data = null,
				message = "آیدی کاربر را وارد کنید",
				is_success = false,
				response_code = 400
			});
		var user = await _unitOfWork.UserRepository.GetUser(src.user_id);
		if (user == null)
			return NotFound(new ResponseDto<TicketDto>()
			{
				data = null,
				message = "کاربر یافت نشد",
				is_success = false,
				response_code = 404
			});
		var reply = new TicketReply()
		{
			ticket_id = src.ticket_id,
			message = src.message,
			user_id = src.user_id,
			created_at = DateTime.Now,
			updated_at = DateTime.Now,
			is_admin = true,
			slug = SlugHelper.GenerateSlug(src.message + Guid.NewGuid().ToString()),
		};

		if(src.picture_file != null)
		{
			// Define the directory for uploads 
			var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "images");

			// Create directory if not Exist
			if (!Directory.Exists(uploadPath))
			{
				Directory.CreateDirectory(uploadPath);
			}

			// Build file name
			var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(src.picture_file.FileName);
			var imagePath = Path.Combine(uploadPath, newFileName);

			// Save Image
			using (var stream = new FileStream(imagePath, FileMode.Create))
			{
				await src.picture_file.CopyToAsync(stream);
			}
			reply.picture = imagePath;
		}

		await _unitOfWork.TicketReplyRepository.AddAsync(reply);

		// به‌روزرسانی وضعیت تیکت
		ticket.status = TicketStatus.Answered;
		_unitOfWork.TicketRepository.Update(ticket);

		await _unitOfWork.CommitAsync();

		return Ok(new ResponseDto<TicketReply>()
		{
			data = reply,
			is_success = true,
			message = "پاسخ ثبت شد",
			response_code = 201
		});
	}

	[HttpPost]
	[Route("close")]
	public async Task<IActionResult> Close([FromForm] long ticketId)
	{
		var ticket = await _unitOfWork.TicketRepository.Get(ticketId);
		if (ticket == null)
			return NotFound(new ResponseDto<TicketDto>()
			{
				data = null,
				message = "تیکت یافت نشد",
				is_success= false,
				response_code = 404
			});

		ticket.status = TicketStatus.Closed;
		_unitOfWork.TicketRepository.Update(ticket);
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<TicketDto>()
		{
			data = null,
			message = "تیکت بسته شد",
			is_success = true,
			response_code = 204
		});
	}


	[HttpPost]
	[Route("delete")]
	public async Task<IActionResult> Delete([FromForm] long ticketId)
	{
		var ticket = await _unitOfWork.TicketRepository.Get(ticketId);
		if (ticket == null)
			return NotFound(new ResponseDto<TicketDto>()
			{
				data = null,
				message = "تیکت یافت نشد",
				is_success = false,
				response_code = 404
			});

		_unitOfWork.TicketRepository.Remove(ticket);
		await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<TicketDto>()
		{
			data = null,
			message = "تیکت حذف شد",
			is_success = true,
			response_code = 204
		});
	}
}

