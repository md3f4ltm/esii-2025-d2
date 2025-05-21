// esii-2025-d2/Controllers/TalentSkillController.cs
using esii_2025_d2.Models;
using esii_2025_d2.Data; // Namespace for your DbContext
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
[Authorize]
public class TalentSkillController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TalentSkillController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/TalentSkill
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TalentSkill>>> GetTalentSkills() // Method name pluralized
    {
        // Use English DbSet name
        return await _context.TalentSkills.ToListAsync();
    }

    // GET: api/TalentSkill/5/10 (using TalentId then SkillId)
    [HttpGet("{talentId}/{skillId}")] // Updated route parameters
    public async Task<ActionResult<TalentSkill>> GetTalentSkill(int talentId, int skillId)
    {
        // Use English property names for composite key lookup
        var talentSkill = await _context.TalentSkills
            .Include(ts => ts.Talent) // Optional include
            .Include(ts => ts.Skill)  // Optional include
            .FirstOrDefaultAsync(ts => ts.TalentId == talentId && ts.SkillId == skillId);

        if (talentSkill == null)
        {
            return NotFound();
        }

        return talentSkill;
    }
    
    // GET: api/TalentSkill/talent/5
    [HttpGet("talent/{talentId}")]
    public async Task<ActionResult<IEnumerable<TalentSkill>>> GetSkillsByTalent(int talentId)
    {
        // Get the current user's ID
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User not authenticated." });
        }

        // Check if talent exists and belongs to the current user
        var talent = await _context.Talents.FirstOrDefaultAsync(t => t.Id == talentId && t.UserId == userId);
        if (talent == null)
        {
            return NotFound(new { message = $"Talent with ID {talentId} not found or does not belong to the current user." });
        }

        // Get all skills for this talent
        var talentSkills = await _context.TalentSkills
            .Where(ts => ts.TalentId == talentId)
            .Include(ts => ts.Skill)
            .ToListAsync();

        return talentSkills;
    }

    // POST: api/TalentSkill
    [HttpPost]
    public async Task<ActionResult<TalentSkill>> CreateTalentSkill(TalentSkill newTalentSkill)
    {
        // Get the current user's ID
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User not authenticated." });
        }
        
        // Check if talent exists and belongs to the current user
        var talent = await _context.Talents.FirstOrDefaultAsync(t => t.Id == newTalentSkill.TalentId && t.UserId == userId);
        if (talent == null)
        {
            return BadRequest(new { message = $"Talent with ID {newTalentSkill.TalentId} not found or does not belong to the current user." });
        }

        if (!await _context.Skills.AnyAsync(s => s.Id == newTalentSkill.SkillId)) // Use Skills DbSet, Id property
            return BadRequest(new { message = $"Skill with ID {newTalentSkill.SkillId} not found." });

        // Optional: Check if the combination already exists
        if (await _context.TalentSkills.AnyAsync(ts => ts.TalentId == newTalentSkill.TalentId && ts.SkillId == newTalentSkill.SkillId))
        {
             return Conflict(new { message = "This talent already has this skill assigned." });
        }


        _context.TalentSkills.Add(newTalentSkill);
        try
        {
            await _context.SaveChangesAsync();
        }
         catch (DbUpdateException /* ex */)
        {
             // Log exception details - Could be unique constraint if check above is removed
            return BadRequest(new { message = "Failed to assign skill to talent. The combination might already exist or other data issues." });
        }


        // Use English property names in route values
        return CreatedAtAction(nameof(GetTalentSkill), new { talentId = newTalentSkill.TalentId, skillId = newTalentSkill.SkillId }, newTalentSkill);
    }

    // PUT: api/TalentSkill/5/10 (using TalentId then SkillId)
    [HttpPut("{talentId}/{skillId}")] // Updated route parameters
    public async Task<IActionResult> UpdateTalentSkill(int talentId, int skillId, TalentSkill updatedTalentSkill)
    {
        // Get the current user's ID
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User not authenticated." });
        }
        
        // Use English property names for check
        if (talentId != updatedTalentSkill.TalentId || skillId != updatedTalentSkill.SkillId)
        {
            return BadRequest("Route IDs do not match payload IDs.");
        }

        // Check if talent belongs to the current user
        var talent = await _context.Talents.FirstOrDefaultAsync(t => t.Id == talentId && t.UserId == userId);
        if (talent == null)
        {
            return BadRequest(new { message = $"Talent with ID {talentId} not found or does not belong to the current user." });
        }

        // Find the existing entity based on the composite key
        var existingEntity = await _context.TalentSkills.FindAsync(talentId, skillId);
        if (existingEntity == null)
        {
             // Decide if PUT should create or return NotFound. Typically NotFound.
             return NotFound();
        }

        // Update only the non-key properties (like YearsOfExperience)
        _context.Entry(existingEntity).CurrentValues.SetValues(updatedTalentSkill);
        // Or more specifically: existingEntity.YearsOfExperience = updatedTalentSkill.YearsOfExperience;
        // _context.Entry(existingEntity).State = EntityState.Modified; // Not needed if using SetValues on tracked entity

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Check existence again for concurrency
             if (!await TalentSkillExists(talentId, skillId))
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
             return BadRequest(new { message = "Failed to update talent skill assignment." });
        }

        return NoContent();
    }

    // DELETE: api/TalentSkill/5/10 (using TalentId then SkillId)
    [HttpDelete("{talentId}/{skillId}")] // Updated route parameters
    public async Task<IActionResult> DeleteTalentSkill(int talentId, int skillId)
    {
        // Get the current user's ID
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User not authenticated." });
        }
        
        // Check if talent belongs to the current user
        var talent = await _context.Talents.FirstOrDefaultAsync(t => t.Id == talentId && t.UserId == userId);
        if (talent == null)
        {
            return BadRequest(new { message = $"Talent with ID {talentId} not found or does not belong to the current user." });
        }

        // Find using composite key with English property names
        var talentSkill = await _context.TalentSkills.FindAsync(talentId, skillId);
        if (talentSkill == null)
        {
            return NotFound();
        }

        _context.TalentSkills.Remove(talentSkill);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> TalentSkillExists(int talentId, int skillId) // Make async if using FindAsync
    {
        // Check using English property names
        // FindAsync is efficient for PK lookups
        return await _context.TalentSkills.FindAsync(talentId, skillId) != null;
        // Or: return _context.TalentSkills.Any(ts => ts.TalentId == talentId && ts.SkillId == skillId);
    }
}
