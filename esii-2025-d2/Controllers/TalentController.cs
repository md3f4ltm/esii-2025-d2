// esii-2025-d2/Controllers/TalentController.cs
using esii_2025_d2.Models;
using esii_2025_d2.Data; // Namespace for your DbContext and User
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
public class TalentController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TalentController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Talent
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Talent>>> GetTalents() // Method name pluralized
    {
        // Use English DbSet name
        return await _context.Talents.ToListAsync();
    }

    // GET: api/Talent/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Talent>> GetTalent(int id)
    {
        // Use English DbSet name and Id property
        // Optional: Include related data if needed on GET by ID
        var talent = await _context.Talents
            // .Include(t => t.TalentCategory)
            // .Include(t => t.User)
            // .Include(t => t.Experiences)
            // .Include(t => t.TalentSkills).ThenInclude(ts => ts.Skill) // Example nested include
            .FirstOrDefaultAsync(t => t.Id == id);


        if (talent == null)
        {
            return NotFound();
        }

        return talent;
    }

    // POST: api/Talent
    [HttpPost]
    public async Task<ActionResult<Talent>> CreateTalent(Talent newTalent)
    {
        // Validate foreign keys
         if (newTalent.TalentCategoryId.HasValue)
        {
             if (!await _context.TalentCategories.AnyAsync(ct => ct.Id == newTalent.TalentCategoryId))
                return BadRequest(new { message = $"Talent Category with ID {newTalent.TalentCategoryId} not found." });
        }
        // else { /* Handle null if needed */ }

        if (!await _context.Users.AnyAsync(u => u.Id == newTalent.UserId)) // Use Users DbSet, Id property
             return BadRequest(new { message = $"User with ID {newTalent.UserId} not found." });


        _context.Talents.Add(newTalent);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException /* ex */)
        {
            // Log exception details
             return BadRequest(new { message = "Failed to create talent. Check related data." });
        }


        return CreatedAtAction(nameof(GetTalent), new { id = newTalent.Id }, newTalent); // Use Id
    }

    // PUT: api/Talent/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTalent(int id, Talent updatedTalent)
    {
        if (id != updatedTalent.Id) // Use Id
        {
            return BadRequest("Talent ID mismatch.");
        }

        // Re-validate foreign keys
        if (updatedTalent.TalentCategoryId.HasValue)
        {
             if (!await _context.TalentCategories.AnyAsync(ct => ct.Id == updatedTalent.TalentCategoryId))
                return BadRequest(new { message = $"Talent Category with ID {updatedTalent.TalentCategoryId} not found." });
        }
         // else { /* Handle null if needed */ }

         if (!await _context.Users.AnyAsync(u => u.Id == updatedTalent.UserId))
             return BadRequest(new { message = $"User with ID {updatedTalent.UserId} not found." });

        _context.Entry(updatedTalent).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TalentExists(id))
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
            return BadRequest(new { message = "Failed to update talent. Check related data." });
        }


        return NoContent();
    }

    // DELETE: api/Talent/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTalent(int id)
    {
        // Include dependent entities that need to be removed first
        var talent = await _context.Talents
            .Include(t => t.Experiences) // Use English navigation property
            .Include(t => t.TalentSkills) // Use English navigation property
            .FirstOrDefaultAsync(t => t.Id == id); // Use Id

        if (talent == null)
        {
            return NotFound();
        }

        // Remove dependent records first (using English names)
        if (talent.Experiences != null && talent.Experiences.Any())
        {
            _context.Experiences.RemoveRange(talent.Experiences); // Use English DbSet name
        }

        if (talent.TalentSkills != null && talent.TalentSkills.Any())
        {
            _context.TalentSkills.RemoveRange(talent.TalentSkills); // Use English DbSet name
        }

        _context.Talents.Remove(talent);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TalentExists(int id)
    {
        return _context.Talents.Any(t => t.Id == id); // Use Id
    }
}
