using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.Models;

namespace StudentApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClassController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClassController(AppDbContext context)
    {
        _context = context;
    }

    // GET api/class
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Class>>> GetAll()
    {
        return await _context.Classes.ToListAsync();
    }

    // GET api/class/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Class>> GetById(int id)
    {
        var cls = await _context.Classes.FindAsync(id);
        if (cls is null)
            return NotFound();

        return cls;
    }

    // POST api/class
    [HttpPost]
    public async Task<ActionResult<Class>> Create(Class cls)
    {
        cls.Id = 0;
        _context.Classes.Add(cls);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = cls.Id }, cls);
    }

    // PUT api/class/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Class cls)
    {
        if (id != cls.Id)
            return BadRequest();

        _context.Entry(cls).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Classes.AnyAsync(c => c.Id == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE api/class/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cls = await _context.Classes.FindAsync(id);
        if (cls is null)
            return NotFound();

        _context.Classes.Remove(cls);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
