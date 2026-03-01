using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.Models;

namespace StudentApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StudentClassXrefController : ControllerBase
{
    private readonly AppDbContext _context;

    public StudentClassXrefController(AppDbContext context)
    {
        _context = context;
    }

    // GET api/studentclassxref
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentClassXref>>> GetAll()
    {
        return await _context.StudentClassXrefs.ToListAsync();
    }

    // GET api/studentclassxref/student/5  → all Classes enrolled by this student
    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<IEnumerable<Class>>> GetClassesByStudent(int studentId)
    {
        if (!await _context.Students.AnyAsync(s => s.Id == studentId))
            return NotFound("Student not found.");

        var classes = await _context.StudentClassXrefs
            .Where(x => x.StudentId == studentId)
            .Join(_context.Classes,
                  x => x.ClassId,
                  c => c.Id,
                  (_, c) => c)
            .ToListAsync();

        return classes;
    }

    // GET api/studentclassxref/class/5  → all Students enrolled in this class
    [HttpGet("class/{classId}")]
    public async Task<ActionResult<IEnumerable<Student>>> GetStudentsByClass(int classId)
    {
        if (!await _context.Classes.AnyAsync(c => c.Id == classId))
            return NotFound("Class not found.");

        var students = await _context.StudentClassXrefs
            .Where(x => x.ClassId == classId)
            .Join(_context.Students,
                  x => x.StudentId,
                  s => s.Id,
                  (_, s) => s)
            .ToListAsync();

        return students;
    }

    // POST api/studentclassxref  → enroll a student in a class
    [HttpPost]
    public async Task<IActionResult> Enroll(StudentClassXref xref)
    {
        if (!await _context.Students.AnyAsync(s => s.Id == xref.StudentId))
            return NotFound("Student not found.");

        if (!await _context.Classes.AnyAsync(c => c.Id == xref.ClassId))
            return NotFound("Class not found.");

        bool alreadyEnrolled = await _context.StudentClassXrefs
            .AnyAsync(x => x.StudentId == xref.StudentId && x.ClassId == xref.ClassId);

        if (alreadyEnrolled)
            return Conflict("Student is already enrolled in this class.");

        _context.StudentClassXrefs.Add(xref);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), xref);
    }

    // DELETE api/studentclassxref/5/3  → unenroll student 5 from class 3
    [HttpDelete("{studentId}/{classId}")]
    public async Task<IActionResult> Unenroll(int studentId, int classId)
    {
        var xref = await _context.StudentClassXrefs
            .FindAsync(studentId, classId);

        if (xref is null)
            return NotFound();

        _context.StudentClassXrefs.Remove(xref);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
