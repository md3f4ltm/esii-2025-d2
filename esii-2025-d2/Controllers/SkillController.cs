// esii-2025-d2/Controllers/SkillController.cs
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
public class SkillController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SkillController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Skill
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Skill>>> GetSkills() // Method name pluralized
    {
        return await _context.Skills.ToListAsync(); // DbSet name is conventional
    }

    // GET: api/Skill/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Skill>> GetSkill(int id)
    {
        var skill = await _context.Skills.FindAsync(id); // Use Id

        if (skill == null)
        {
            return NotFound();
        }
        return skill;
    }

    // GET: api/Skill/{id}/inuse
    [HttpGet("{id}/inuse")]
    public async Task<ActionResult<bool>> IsSkillInUse(int id)
    {
        if (!await _context.Skills.AnyAsync(s => s.Id == id))
        {
            return NotFound(new { message = $"Skill with ID {id} not found." });
        }

        // Check if skill is associated with any talents
        bool isInUse = await _context.TalentSkills.AnyAsync(ts => ts.SkillId == id);

        // You could also check if it's in use by job proposals
        isInUse = isInUse || await _context.JobProposals.AnyAsync(jp => jp.SkillId == id);

        return isInUse;
    }


    // POST: api/Skill
    [HttpPost]
    public async Task<ActionResult<Skill>> CreateSkill(Skill newSkill)
    {
        // Use Name property for conflict check
        if (await _context.Skills.AnyAsync(s => s.Name == newSkill.Name))
        {
            return Conflict(new { message = "A skill with this name already exists." });
        }

        _context.Skills.Add(newSkill);
        await _context.SaveChangesAsync();

        // Use Id property in route values
        return CreatedAtAction(nameof(GetSkill), new { id = newSkill.Id }, newSkill);
    }


    // PUT: api/Skill/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSkill(int id, Skill updatedSkill)
    {
        if (id != updatedSkill.Id) // Use Id
        {
            return BadRequest("Skill ID mismatch.");
        }

         // Optional: Check for name conflict if name is unique and changed
        // if (_context.Skills.Any(s => s.Name == updatedSkill.Name && s.Id != updatedSkill.Id))
        // {
        //     return Conflict(new { message = "Another skill with this name already exists." });
        // }

        _context.Entry(updatedSkill).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SkillExists(id))
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

    // DELETE: api/Skill/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        var skill = await _context.Skills.FindAsync(id); // Use Id
        if (skill == null)
        {
            return NotFound();
        }

        // Check if skill is in use before deleting
        bool isInUse = await _context.TalentSkills.AnyAsync(ts => ts.SkillId == id);
        isInUse = isInUse || await _context.JobProposals.AnyAsync(jp => jp.SkillId == id);

        if (isInUse)
        {
            return BadRequest(new { message = "Cannot delete this skill because it is associated with talents or job proposals." });
        }

        _context.Skills.Remove(skill);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SkillExists(int id)
    {
        return _context.Skills.Any(s => s.Id == id); // Use Id
    }
}
