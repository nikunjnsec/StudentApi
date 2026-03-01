using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.Models;

namespace StudentApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly AppDbContext _context;

    public StudentController(AppDbContext context)
    {
        _context = context;
    }

    // GET api/student
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Student>>> GetAll()
    {
        return await _context.Students.ToListAsync();
    }

    // GET api/student/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Student>> GetById(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student is null)
            return NotFound();

        return student;
    }

    // POST api/student
    [HttpPost]
    public async Task<ActionResult<Student>> Create(Student student)
    {
        student.Id = 0; // ignore any client-supplied id; let SQL Server generate it
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
    }

    // PUT api/student/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Student student)
    {
        if (id != student.Id)
            return BadRequest();

        _context.Entry(student).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Students.AnyAsync(s => s.Id == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE api/student/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student is null)
            return NotFound();

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
