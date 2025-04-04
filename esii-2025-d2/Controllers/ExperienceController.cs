// esii-2025-d2/Controllers/ExperienceController.cs
using esii_2025_d2.Models;
using esii_2025_d2.Data; // Namespace for your DbContext
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// using Microsoft.AspNetCore.Authorization; // Uncomment if needed

namespace esii_2025_d2.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize] // Uncomment if needed
public class ExperienceController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ExperienceController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Experience
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Experience>>> GetExperiences()
    {
        return await _context.Experiences.ToListAsync(); // Use English DbSet name
    }

    // GET: api/Experience/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Experience>> GetExperience(int id)
    {
        var experience = await _context.Experiences
            .Include(e => e.Talent) // Include related Talent (using English nav prop)
            .SingleOrDefaultAsync(e => e.Id == id); // Use Id

        if (experience == null)
        {
            return NotFound();
        }

        return experience;
    }

     // POST: api/Experience
    [HttpPost]
    public async Task<ActionResult<Experience>> CreateExperience(Experience newExperience)
    {
        // Validate that the TalentId exists
        if (await _context.Talents.FindAsync(newExperience.TalentId) == null) // Use English DbSet and TalentId
        {
            return BadRequest(new { message = $"Talent with ID {newExperience.TalentId} not found." });
        }

        _context.Experiences.Add(newExperience);
        try
        {
             await _context.SaveChangesAsync();
        }
        catch (DbUpdateException /* ex */)
        {
             // Log exception details
            return BadRequest(new { message = "Failed to create experience. Check related data." });
        }

        return CreatedAtAction(nameof(GetExperience), new { id = newExperience.Id }, newExperience); // Use Id
    }

    // PUT: api/Experience/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExperience(int id, Experience updatedExperience)
    {
        if (id != updatedExperience.Id) // Use Id
        {
            return BadRequest("Experience ID mismatch.");
        }

        // Validate that the TalentId exists
        if (await _context.Talents.FindAsync(updatedExperience.TalentId) == null) // Use English DbSet and TalentId
        {
            return BadRequest(new { message = $"Talent with ID {updatedExperience.TalentId} not found." });
        }


        _context.Entry(updatedExperience).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ExperienceExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
         catch (DbUpdateException /* ex */)
        {
            // Log exception details
            return BadRequest(new { message = "Failed to update experience. Check related data." });
        }

        return NoContent();
    }

    // DELETE: api/Experience/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExperience(int id)
    {
        var experience = await _context.Experiences.FindAsync(id); // Find by Id
        if (experience == null)
        {
            return NotFound();
        }

        _context.Experiences.Remove(experience);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ExperienceExists(int id)
    {
        return _context.Experiences.Any(e => e.Id == id); // Use Id
    }
}
