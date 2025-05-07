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
    [HasPermission("Blog.Index")]
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
    [HasPermission("Blog.Create")]
    public async Task<IActionResult> Create([FromForm] BlogDto src)
    {
        if (!ModelState.IsValid)
        {
            var error = string.Join(" | ", ModelState.Values
                   .SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage));
            return BadRequest(error);
        }
        if (src.BlogText == "<p><br></p>")
        {
            var error = "لطفا مقدار متن خبر را وارد کنید";
            return BadRequest(error);
        }
        if (src.ImageFile == null)
        {
            var error = "لطفا تصویر شاخص را انتخاب کنید";
            return BadRequest(error);
        }
        if (await _unitOfWork.BlogRepository.ExistsAsync(x => x.Name == src.Name))
        {
            var error = "بلاگ با این نام وجود دارد";
            return BadRequest(error);
        }
        var slug = src.Slug ?? SlugHelper.GenerateSlug(src.Name);
        if (await _unitOfWork.BlogRepository.ExistsAsync(x => x.Slug == slug))
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
        var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(src.ImageFile.FileName);
        var imagePath = Path.Combine(uploadPath, newFileName);

        // Save Image
        using (var stream = new FileStream(imagePath, FileMode.Create))
        {
            await src.ImageFile.CopyToAsync(stream);
        }

        await _unitOfWork.BlogRepository.AddAsync(new()
        {
            UpdatedAt = DateTime.Now,
            CreatedAt = DateTime.Now,
            Name = src.Name,
            Slug = slug,
            BlogText = src.BlogText,
            BlogCategoryId = (int)src.BlogCategoryId,
            Image = "images/" + newFileName,
            ImageAlt = src.ImageAlt,
            ShortDescription = src.ShortDescription,
            ShowBlog = src.ShowBlog,
            KeyWords = src.KeyWords
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
    [HasPermission("Blog.Edit")]
    public async Task<IActionResult> Edit([FromForm] BlogDto src)
    {
        if (!ModelState.IsValid)
        {
            var error = string.Join(" | ", ModelState.Values
                   .SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage));
            return BadRequest(error);
        }
        if (src.BlogText == "<p><br></p>")
        {
            var error = "لطفا مقدار متن خبر را وارد کنید";
            return BadRequest(error);
        }
        var entity = await _unitOfWork.BlogRepository.Get(src.Id);
        if (entity == null)
            return NotFound();
        if (await _unitOfWork.BlogRepository.ExistsAsync(x => x.Name == src.Name) && entity.Name != src.Name)
        {
            var error = "بلاگ با این نام وجود دارد";
            return BadRequest(error);
        }
        var slug = src.Slug ?? SlugHelper.GenerateSlug(src.Name);
        if (await _unitOfWork.BlogRepository.ExistsAsync(x => x.Slug == slug) && entity.Slug != slug)
        {
            var error = "بلاگ با این نامک وجود دارد";
            return BadRequest(error);
        }


        if (src.ImageFile != null)
        {
            // Delete old file
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "images", entity.Image);
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
            var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(src.ImageFile.FileName);
            // file path
            var image = Path.Combine(uploadPath, newFileName);
            using (var stream = new FileStream(image, FileMode.Create))
            {
                await src.ImageFile.CopyToAsync(stream);
            }

            entity.Image = "images/" + newFileName;
        }

        var blog = await _unitOfWork.BlogRepository.Get(src.Id);
        if (blog == null)
            return NotFound();

        var oldImages = ExtractImageSources(blog.BlogText);
        var newImages = ExtractImageSources(src.BlogText);

        var imagesToDelete = oldImages.Except(newImages).ToList();

        foreach (var imgUrl in imagesToDelete)
        {
            // فرض بر اینکه imgUrl به شکل "/uploads/blog/abc.jpg" هست
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", imgUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        entity.UpdatedAt = DateTime.Now;
        entity.Name = src.Name;
        entity.Slug = slug;
        entity.BlogText = src.BlogText;
        entity.BlogCategoryId = (int)src.BlogCategoryId;
        entity.ImageAlt = src.ImageAlt;
        entity.ShortDescription = src.ShortDescription;
        entity.ShowBlog = src.ShowBlog;
        entity.KeyWords = src.KeyWords;
        _unitOfWork.BlogRepository.Update(entity);
        await _unitOfWork.CommitAsync();
        return Ok();
    }

    [HttpPost]
    [Route("delete-blog/{id}")]
    [HasPermission("Blog.Delete")]
    public async Task<IActionResult> Delete([FromRoute] long id)
    {
        var entity = await _unitOfWork.BlogRepository.Get(id);
        if (entity == null)
            return NotFound();
        if (System.IO.File.Exists(entity.Image))
        {
            System.IO.File.Delete(entity.Image);
        }
        var imagesToDelete = ExtractImageSources(entity.BlogText);

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
    [HasPermission("Blog.Detail")]
    public async Task<IActionResult> Detail([FromRoute] string slug)
    {
        var entity = await _unitOfWork.BlogRepository.Get(slug);
        if (entity == null) return NotFound();

        return Ok(new BlogDto()
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Slug = slug,
            Name = entity.Name,
            BlogCategoryId = entity.BlogCategoryId,
            BlogText = entity.BlogText,
            Image = entity.Image,
            ImageAlt = entity.ImageAlt,
            ShortDescription = entity.ShortDescription,
            ShowBlog = entity.ShowBlog,
            KeyWords = entity.KeyWords,
            BlogCategory = new BlogCategoryDto()
            {
                CreatedAt = entity.BlogCategory.CreatedAt,
                UpdatedAt = entity.BlogCategory.UpdatedAt,
                Slug = entity.BlogCategory.Slug,
                Description = entity.BlogCategory.Description,
                Name = entity.BlogCategory.Name,
                Id = entity.BlogCategory.Id,
            }
        });
    }

    [HttpPost]
    [Route("check-show-blog/{id}")]
    [HasPermission("Blog.CheckShowBlog")]
    public async Task<IActionResult> CheckShowBlog([FromRoute] long id)
    {
        var entity = await _unitOfWork.BlogRepository.Get(id);
        if (entity == null) return NotFound();
        entity.ShowBlog = !entity.ShowBlog;
        _unitOfWork.BlogRepository.Update(entity);
        await _unitOfWork.CommitAsync();

        return Ok();
    }

    [HttpPost]
    [Route("upload-image")]
    [HasPermission("Blog.UploadImage")]
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

        var fileUrl = Url.Content($"/images/{fileName}");
        return Ok(new { url = fileUrl });
    }
}
