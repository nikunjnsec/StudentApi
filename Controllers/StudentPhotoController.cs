using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.Models;

namespace StudentApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StudentPhotoController : ControllerBase
{
    private readonly AppDbContext _context;

    public StudentPhotoController(AppDbContext context)
    {
        _context = context;
    }

    // GET api/studentphoto/5  — AllowAnonymous so <img src="..."> works in the browser
    [AllowAnonymous]
    [HttpGet("{studentId}")]
    public async Task<IActionResult> GetPhoto(int studentId)
    {
        var photo = await _context.StudentPhotos.FindAsync(studentId);
        if (photo is null) return NotFound();

        return File(photo.PhotoData, photo.ContentType);
    }

    // POST api/studentphoto/5  — upload or replace photo
    [HttpPost("{studentId}")]
    public async Task<IActionResult> UploadPhoto(int studentId, IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file uploaded.");

        if (!file.ContentType.StartsWith("image/"))
            return BadRequest("File must be an image.");

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("Image must be under 5 MB.");

        if (!await _context.Students.AnyAsync(s => s.Id == studentId))
            return NotFound("Student not found.");

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var data = ms.ToArray();

        var existing = await _context.StudentPhotos.FindAsync(studentId);
        if (existing is not null)
        {
            existing.FileName = file.FileName;
            existing.ContentType = file.ContentType;
            existing.PhotoData = data;
        }
        else
        {
            _context.StudentPhotos.Add(new StudentPhoto
            {
                StudentId = studentId,
                FileName = file.FileName,
                ContentType = file.ContentType,
                PhotoData = data
            });
        }

        await _context.SaveChangesAsync();
        return Ok();
    }

    // DELETE api/studentphoto/5
    [HttpDelete("{studentId}")]
    public async Task<IActionResult> DeletePhoto(int studentId)
    {
        var photo = await _context.StudentPhotos.FindAsync(studentId);
        if (photo is null) return NotFound();

        _context.StudentPhotos.Remove(photo);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
