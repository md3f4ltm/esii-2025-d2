// esii-2025-d2/Controllers/TalentController.cs
using esii_2025_d2.Models;
using esii_2025_d2.Data; // Namespace for your DbContext and User
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace esii_2025_d2.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TalentController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TalentController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Talent
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Talent>>> GetTalents() // Existing method
    {
        // Use English DbSet name
        return await _context.Talents.ToListAsync();
    }

    // *** START: NEW ENDPOINT FOR FEED ***
    // GET: api/Talent/GetAllTalents
    [HttpGet("GetAllTalents")]
    public async Task<ActionResult<IEnumerable<Talent>>> GetAllTalents()
    {
        // Consider adding filtering or pagination for large datasets
        // Also consider which related data to include (e.g., Skills)
        // return await _context.Talents.Include(t => t.TalentSkills).ThenInclude(ts => ts.Skill).ToListAsync();
        return await _context.Talents.ToListAsync(); // Simple version for now
    }
    // *** END: NEW ENDPOINT FOR FEED ***

    // GET: api/Talent/mytalents
    [HttpGet("mytalents")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Talent>>> GetMyTalents()
    {
        // Get the current user's ID from the claims
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User not authenticated or user ID not found in claims." });
        }

        // Find all talents belonging to the current user
        var talents = await _context.Talents
            .Where(t => t.UserId == userId)
            .ToListAsync();

        return talents;
    }

    // GET: api/Talent/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Talent>> GetTalent(int id) // Method name singularized
    {
        // Use English DbSet name
        var talent = await _context.Talents.FindAsync(id);

        if (talent == null)
        {
            return NotFound();
        }

        return talent;
    }

    // PUT: api/Talent/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTalent(int id, Talent talent)
    {
        if (id != talent.Id)
        {
            return BadRequest();
        }

        // Use English DbSet name
        _context.Entry(talent).State = EntityState.Modified;

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

        return NoContent();
    }

    // POST: api/Talent
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Talent>> PostTalent(Talent talent)
    {
        // Use English DbSet name
        _context.Talents.Add(talent);
        await _context.SaveChangesAsync();

        // Use English property name (assuming Id)
        return CreatedAtAction("GetTalent", new { id = talent.Id }, talent);
    }

    // DELETE: api/Talent/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTalent(int id)
    {
        // Use English DbSet name
        var talent = await _context.Talents.FindAsync(id);
        if (talent == null)
        {
            return NotFound();
        }

        // Use English DbSet name
        _context.Talents.Remove(talent);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TalentExists(int id)
    {
        // Use English DbSet name
        return _context.Talents.Any(e => e.Id == id);
    }
}
