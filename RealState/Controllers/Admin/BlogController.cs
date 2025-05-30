using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaelState.Models;
using RealState.Models;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RaelState.Controllers.Admin;
[Route("api/blog")]
[ApiController]
[Authorize(Roles = "Admin,MainAdmin")]
public class BlogController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> Index(string? search_term,
        SortByEnum sort_by = SortByEnum.CreationDate,
        int page = 1,
        int page_size = 10)
    {
        // Set up filter object
        var filter = new DefaultPaginationFilter(page, page_size)
        {
            Keyword = search_term,
            SortBy = sort_by,
        };
        var data = _unitOfWork.BlogRepository.GetPaginated(filter, null);
		return Ok(new ResponseDto<PaginatedList<Blog>>()
		{
			data = data,
			is_success = true,
			message = "",
			response_code = 200
		});
	}

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create([FromForm] BlogDto src)
    {
        if (!ModelState.IsValid)
        {
            var error = string.Join(" | ", ModelState.Values
                   .SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<BlogDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
        if (src.blog_text == "<p><br></p>")
        {
            var error = "لطفا مقدار متن خبر را وارد کنید";
			return BadRequest(new ResponseDto<BlogDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
        if (src.image_file == null)
        {
            var error = "لطفا تصویر شاخص را انتخاب کنید";
			return BadRequest(new ResponseDto<BlogDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
        if (await _unitOfWork.BlogRepository.ExistsAsync(x => x.name == src.name))
        {
            var error = "بلاگ با این نام وجود دارد";
			return BadRequest(new ResponseDto<BlogDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
        var slug = src.slug ?? SlugHelper.GenerateSlug(src.name);
        if (await _unitOfWork.BlogRepository.ExistsAsync(x => x.slug == slug))
        {
            var error = "بلاگ با این نامک وجود دارد";
			return BadRequest(new ResponseDto<BlogDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}

		var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
		if (!Directory.Exists(uploadPath))
			Directory.CreateDirectory(uploadPath);

		var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(src.image_file.FileName);
		var imagePath = Path.Combine(uploadPath, newFileName);

		using (var stream = new FileStream(imagePath, FileMode.Create))
		{
			await src.image_file.CopyToAsync(stream);
		}

		var baseUrl = $"{Request.Scheme}://{Request.Host}";
		var imageUrl = $"{baseUrl}/images/{newFileName}";

		await _unitOfWork.BlogRepository.AddAsync(new()
		{
			updated_at = DateTime.Now,
			created_at = DateTime.Now,
			name = src.name,
			slug = slug,
			blog_text = src.blog_text,
			blog_category_id = src.blog_category_id,
			image = imageUrl,   // ذخیره آدرس URL
			image_alt = src.image_alt,
			description = src.description,
			show_blog = src.show_blog,
			keywords = src.keyWords
		});

		await _unitOfWork.CommitAsync();

		return Ok(new ResponseDto<BlogDto>()
		{
			data = null,
			is_success = true,
			message = "بلاگ با موفقیت ایجاد شد",
			response_code = 201
		});
	}

    private static List<string> ExtractImageSources(string html)
    {
        var imgSrcRegex = new Regex("<img[^>]*src=[\"']([^\"']+)[\"'][^>]*>", RegexOptions.IgnoreCase);
        var matches = imgSrcRegex.Matches(html ?? "");
        return matches.Select(m => m.Groups[1].Value).ToList();
    }


    [HttpPost]
    [Route("edit")]
    public async Task<IActionResult> Edit([FromForm] BlogDto src)
    {
        if (!ModelState.IsValid)
        {
            var error = string.Join(" | ", ModelState.Values
                   .SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage));
			return BadRequest(new ResponseDto<BlogDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
        if (src.blog_text == "<p><br></p>")
        {
            var error = "لطفا مقدار متن خبر را وارد کنید";
			return BadRequest(new ResponseDto<BlogDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
        var entity = await _unitOfWork.BlogRepository.Get(src.id);
        if (entity == null)
            return NotFound();
        if (await _unitOfWork.BlogRepository.ExistsAsync(x => x.name == src.name) && entity.name != src.name)
        {
            var error = "بلاگ با این نام وجود دارد";
			return BadRequest(new ResponseDto<BlogDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}
        var slug = src.slug ?? SlugHelper.GenerateSlug(src.name);
        if (await _unitOfWork.BlogRepository.ExistsAsync(x => x.slug == slug) && entity.slug != slug)
        {
            var error = "بلاگ با این نامک وجود دارد";
			return BadRequest(new ResponseDto<BlogDto>()
			{
				data = null,
				is_success = false,
				message = error,
				response_code = 400
			});
		}


        if (src.image_file != null)
        {
			// Delete old file
			// مثال: https://example.com/images/abc.jpg
			var imageUrl = entity.image;

			// فقط بخش مسیر بعد از دامنه (یعنی /images/abc.jpg)
			var relativePath = new Uri(imageUrl).AbsolutePath.TrimStart('/');

			// ساخت مسیر فیزیکی کامل روی سرور
			var fullImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

			if (System.IO.File.Exists(fullImagePath))
            {
                System.IO.File.Delete(fullImagePath);
            }

            // Define the directory for uploads
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "images");

            // Ensure the directory exists
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Build file name
            var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(src.image_file.FileName);
            // file path
            var image = Path.Combine(uploadPath, newFileName);
            using (var stream = new FileStream(image, FileMode.Create))
            {
                await src.image_file.CopyToAsync(stream);
            }

			var baseUrl = $"{Request.Scheme}://{Request.Host}";
			var imageUrl = $"{baseUrl}/images/{newFileName}";

			entity.image = imageUrl;
        }

        var blog = await _unitOfWork.BlogRepository.Get(src.id);
        if (blog == null)
			return NotFound(new ResponseDto<BlogDto>()
			{
				data = null,
				is_success = false,
				message = "بلاگ با این ایدی پیدا نشد.",
				response_code = 404
			});

		var oldImages = ExtractImageSources(blog.blog_text);
        var newImages = ExtractImageSources(src.blog_text);

        var imagesToDelete = oldImages.Except(newImages).ToList();

        foreach (var imgUrl in imagesToDelete)
        {
            if (System.IO.File.Exists(imgUrl))
            {
                System.IO.File.Delete(imgUrl);
            }
        }

        entity.updated_at = DateTime.Now;
        entity.name = src.name;
        entity.slug = slug;
        entity.blog_text = src.blog_text;
        entity.blog_category_id = src.blog_category_id;
        entity.image_alt = src.image_alt;
        entity.description = src.description;
        entity.show_blog = src.show_blog;
        entity.keywords = src.keyWords;
        _unitOfWork.BlogRepository.Update(entity);
        await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<BlogDto>()
		{
			data = null,
			is_success = true,
			message = "بلاگ با موفقیت ویرایش شد.",
			response_code = 204
		});
	}

    [HttpPost]
    [Route("delete")]
    public async Task<IActionResult> Delete([FromForm] long id)
    {
        var entity = await _unitOfWork.BlogRepository.Get(id);
        if (entity == null)
			return NotFound(new ResponseDto<BlogDto>()
			{
				data = null,
				is_success = false,
				message = "بلاگ با این ایدی پیدا نشد.",
				response_code = 404
			});
		// مثال: https://example.com/images/abc.jpg
		var imageUrl = entity.image;

		// فقط بخش مسیر بعد از دامنه (یعنی /images/abc.jpg)
		var relativePath = new Uri(imageUrl).AbsolutePath.TrimStart('/');

		// ساخت مسیر فیزیکی کامل روی سرور
		var fullImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

		// حذف فایل در صورت وجود
		if (System.IO.File.Exists(fullImagePath))
		{
			System.IO.File.Delete(fullImagePath);
		}
		
        var imagesToDelete = ExtractImageSources(entity.blog_text);

        foreach (var imgUrl in imagesToDelete)
        {
			// فرض بر اینکه imgUrl به شکل "/uploads/blog/abc.jpg" هست
			// مثال: https://example.com/images/abc.jpg
			var imageUrl2 = entity.image;

			// فقط بخش مسیر بعد از دامنه (یعنی /images/abc.jpg)
			var relativePath2 = new Uri(imageUrl2).AbsolutePath.TrimStart('/');

			// ساخت مسیر فیزیکی کامل روی سرور
			var fullImagePath2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

			if (System.IO.File.Exists(fullImagePath2))
            {
                System.IO.File.Delete(fullImagePath2);
            }
        }
        _unitOfWork.BlogRepository.Remove(entity);
        await _unitOfWork.CommitAsync();
		return Ok(new ResponseDto<BlogDto>()
		{
			data = null,
			is_success = true,
			message = "بلاگ با موفقیت حذف شد.",
			response_code = 204
		});
	}

    [HttpGet]
    [Route("read/{slug}")]
    public async Task<IActionResult> Detail([FromRoute] string slug)
    {
        var entity = await _unitOfWork.BlogRepository.Get(slug);
        if (entity == null)
			return NotFound(new ResponseDto<BlogDto>()
			{
				data = null,
				is_success = false,
				message = "بلاگ با این slug پیدا نشد.",
				response_code = 404
			});

		return Ok(new ResponseDto<BlogDto>()
        {
            data = new BlogDto()
			{
				id = entity.id,
				created_at = entity.created_at,
				updated_at = entity.updated_at,
				slug = entity.slug,
				name = entity.name,
				blog_category_id = entity.blog_category_id,
				blog_text = entity.blog_text,
				image = entity.image,
				image_alt = entity.image_alt,
				description = entity.description,
				show_blog = entity.show_blog,
				keyWords = entity.keywords,
				blog_category = new BlogCategoryDto()
				{
					created_at = entity.blog_category.created_at,
					updated_at = entity.blog_category.updated_at,
					slug = entity.blog_category.slug,
					description = entity.blog_category.description,
					name = entity.blog_category.name,
					id = entity.blog_category.id,
				}
			},
            is_success = true,
            message = "",
            response_code = 200
		});
    }

    [HttpPost]
    [Route("check-show")]
    public async Task<IActionResult> CheckShowBlog([FromForm] long id)
    {
        var entity = await _unitOfWork.BlogRepository.Get(id);
        if (entity == null)
			return NotFound(new ResponseDto<BlogDto>()
			{
				data = null,
				is_success = false,
				message = "بلاگ با این ایدی پیدا نشد.",
				response_code = 404
			});
		entity.show_blog = !entity.show_blog;
        _unitOfWork.BlogRepository.Update(entity);
        await _unitOfWork.CommitAsync();

		return Ok(new ResponseDto<BlogDto>()
		{
			data = null,
			is_success = true,
			message = "وضعیت بلاگ تغییر کرد",
			response_code = 204
		});
	}

    [HttpPost]
    [Route("upload-image")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
			return BadRequest(new ResponseDto<BlogDto>()
			{
				data = null,
				is_success = false,
				message = "فایلی وجود ندارد",
				response_code = 400
			});

		var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var ext = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(ext))
		    return BadRequest(new ResponseDto<BlogDto>()
		    {
			    data = null,
			    is_success = false,
			    message = "نوع فایل نامعتبر است",
			    response_code = 400
		    });

		var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "images");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

		var baseUrl = $"{Request.Scheme}://{Request.Host}";
		var imageUrl = $"{baseUrl}/images/{fileName}";
		return Ok(new ResponseDto<string>()
        {
            data = imageUrl,
            is_success = true,
            message = "" ,
            response_code = 200
        });
    }
}
