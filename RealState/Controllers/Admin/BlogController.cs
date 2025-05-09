using DataLayer;
using DataLayer.Assistant.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using RaelState.Assistant;
using RaelState.Models;

namespace RaelState.Controllers.Admin;
[Route("api/blog")]
[ApiController]
public class BlogController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [HttpGet]
    [Route("home")]
    public async Task<IActionResult> Index(string? searchTerm,
        SortByEnum sortBy = SortByEnum.CreationDate,
        int page = 1,
        int pageSize = 10)
    {
        // Set up filter object
        var filter = new DefaultPaginationFilter(page, pageSize)
        {
            Keyword = searchTerm,
            SortBy = sortBy,
        };
        var data = _unitOfWork.BlogCategoryRepository.GetPaginated(filter);
        return Ok(data);
    }

    [HttpPost]
    [Route("create-blog")]
    public async Task<IActionResult> Create([FromForm] BlogDto src)
    {
        if (!ModelState.IsValid)
        {
            var error = string.Join(" | ", ModelState.Values
                   .SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage));
            return BadRequest(error);
        }
        if (src.blog_text == "<p><br></p>")
        {
            var error = "لطفا مقدار متن خبر را وارد کنید";
            return BadRequest(error);
        }
        if (src.image_file == null)
        {
            var error = "لطفا تصویر شاخص را انتخاب کنید";
            return BadRequest(error);
        }
        if (await _unitOfWork.BlogRepository.ExistsAsync(x => x.name == src.name))
        {
            var error = "بلاگ با این نام وجود دارد";
            return BadRequest(error);
        }
        var slug = src.slug ?? SlugHelper.GenerateSlug(src.name);
        if (await _unitOfWork.BlogRepository.ExistsAsync(x => x.slug == slug))
        {
            var error = "بلاگ با این نامک وجود دارد";
            return BadRequest(error);
        }

        // Define the directory for uploads 
        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "images");

        // Create directory if not Exist
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        // Build file name
        var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(src.image_file.FileName);
        var imagePath = Path.Combine(uploadPath, newFileName);

        // Save Image
        using (var stream = new FileStream(imagePath, FileMode.Create))
        {
            await src.image_file.CopyToAsync(stream);
        }

        await _unitOfWork.BlogRepository.AddAsync(new()
        {
            updated_at = DateTime.Now,
            created_at = DateTime.Now,
            name = src.name,
            slug = slug,
            blog_text = src.blog_text,
            blog_category_id = src.blog_category_id,
            image = imagePath,
            image_alt = src.image_alt,
            short_description = src.short_description,
            show_blog = src.show_blog,
            keywords = src.keyWords
        });

        await _unitOfWork.CommitAsync();
        return Ok();
    }

    private static List<string> ExtractImageSources(string html)
    {
        var imgSrcRegex = new Regex("<img[^>]*src=[\"']([^\"']+)[\"'][^>]*>", RegexOptions.IgnoreCase);
        var matches = imgSrcRegex.Matches(html ?? "");
        return matches.Select(m => m.Groups[1].Value).ToList();
    }


    [HttpPost]
    [Route("blog-edit")]
    public async Task<IActionResult> Edit([FromForm] BlogDto src)
    {
        if (!ModelState.IsValid)
        {
            var error = string.Join(" | ", ModelState.Values
                   .SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage));
            return BadRequest(error);
        }
        if (src.blog_text == "<p><br></p>")
        {
            var error = "لطفا مقدار متن خبر را وارد کنید";
            return BadRequest(error);
        }
        var entity = await _unitOfWork.BlogRepository.Get(src.id);
        if (entity == null)
            return NotFound();
        if (await _unitOfWork.BlogRepository.ExistsAsync(x => x.name == src.name) && entity.name != src.name)
        {
            var error = "بلاگ با این نام وجود دارد";
            return BadRequest(error);
        }
        var slug = src.slug ?? SlugHelper.GenerateSlug(src.name);
        if (await _unitOfWork.BlogRepository.ExistsAsync(x => x.slug == slug) && entity.slug != slug)
        {
            var error = "بلاگ با این نامک وجود دارد";
            return BadRequest(error);
        }


        if (src.image_file != null)
        {
            // Delete old file
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), entity.image);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
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

            entity.image = image;
        }

        var blog = await _unitOfWork.BlogRepository.Get(src.id);
        if (blog == null)
            return NotFound();

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
        entity.short_description = src.short_description;
        entity.show_blog = src.show_blog;
        entity.keywords = src.keyWords;
        _unitOfWork.BlogRepository.Update(entity);
        await _unitOfWork.CommitAsync();
        return Ok();
    }

    [HttpPost]
    [Route("delete-blog/{id}")]
    public async Task<IActionResult> Delete([FromRoute] long id)
    {
        var entity = await _unitOfWork.BlogRepository.Get(id);
        if (entity == null)
            return NotFound();
        if (System.IO.File.Exists(entity.image))
        {
            System.IO.File.Delete(entity.image);
        }
        var imagesToDelete = ExtractImageSources(entity.blog_text);

        foreach (var imgUrl in imagesToDelete)
        {
            // فرض بر اینکه imgUrl به شکل "/uploads/blog/abc.jpg" هست
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", imgUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
        _unitOfWork.BlogRepository.Remove(entity);
        await _unitOfWork.CommitAsync();
        return Ok();
    }

    [HttpGet]
    [Route("blog-detail/{slug}")]
    public async Task<IActionResult> Detail([FromRoute] string slug)
    {
        var entity = await _unitOfWork.BlogRepository.Get(slug);
        if (entity == null) return NotFound();

        return Ok(new BlogDto()
        {
            id = entity.id,
            created_at = entity.created_at,
            updated_at = entity.updated_at,
            slug = slug,
            name = entity.name,
            blog_category_id = entity.blog_category_id,
            blog_text = entity.blog_text,
            image = entity.image,
            image_alt = entity.image_alt,
            short_description = entity.short_description,
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
        });
    }

    [HttpPost]
    [Route("check-show-blog/{id}")]
    public async Task<IActionResult> CheckShowBlog([FromRoute] long id)
    {
        var entity = await _unitOfWork.BlogRepository.Get(id);
        if (entity == null) return NotFound();
        entity.show_blog = !entity.show_blog;
        _unitOfWork.BlogRepository.Update(entity);
        await _unitOfWork.CommitAsync();

        return Ok();
    }

    [HttpPost]
    [Route("upload-image")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var ext = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(ext))
            return BadRequest("نوع فایل نامعتبر است");

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "images");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileUrl = Url.Content(filePath);
        return Ok(new { url = fileUrl });
    }
}
