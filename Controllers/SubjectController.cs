using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.Models;

namespace StudentApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SubjectController : ControllerBase
{
    private readonly AppDbContext _context;

    public SubjectController(AppDbContext context)
    {
        _context = context;
    }

    // GET api/subject
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Subject>>> GetAll()
    {
        return await _context.Subjects.ToListAsync();
    }

    // GET api/subject/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Subject>> GetById(int id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject is null) return NotFound();
        return subject;
    }

    // GET api/subject/class/5 → subject for a specific class
    [HttpGet("class/{classId}")]
    public async Task<ActionResult<Subject>> GetByClass(int classId)
    {
        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.ClassId == classId);
        if (subject is null) return NotFound();
        return subject;
    }

    // POST api/subject
    [HttpPost]
    public async Task<ActionResult<Subject>> Create(Subject subject)
    {
        subject.Id = 0;
        _context.Subjects.Add(subject);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("A subject for this class already exists.");
        }
        return CreatedAtAction(nameof(GetById), new { id = subject.Id }, subject);
    }

    // PUT api/subject/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Subject subject)
    {
        if (id != subject.Id) return BadRequest();

        _context.Entry(subject).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Subjects.AnyAsync(s => s.Id == id)) return NotFound();
            throw;
        }
        catch (DbUpdateException)
        {
            return Conflict("A subject for this class already exists.");
        }
        return NoContent();
    }

    // DELETE api/subject/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject is null) return NotFound();
        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
