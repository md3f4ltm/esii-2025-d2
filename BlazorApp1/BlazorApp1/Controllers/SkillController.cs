using ESII2025d2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ESII2025d2.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class SkillController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SkillController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Skill>>> GetSkill()
    {
        return await _context.Skills.ToListAsync();
    }
    
    [HttpPost]
    public async Task<ActionResult<Skill>> CreateSkill(Skill novaSkill)
    {
        if (await _context.Skills.AnyAsync(s => s.nome == novaSkill.nome))
        {
            return Conflict("Já existe uma skill com este nome.");
        }

        _context.Skills.Add(novaSkill);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSkill), new { id = novaSkill.cod }, novaSkill);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Skill>> GetSkill(int id)
    {
        var skill = await _context.Skills.FindAsync(id);
        if (skill == null)
        {
            return NotFound();
        }
        return skill;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSkill(int id, Skill skillAtualizada)
    {
        if (id != skillAtualizada.cod)
        {
            return BadRequest("O ID da skill não corresponde.");
        }

        _context.Entry(skillAtualizada).State = EntityState.Modified;

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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        var skill = await _context.Skills.FindAsync(id);
        if (skill == null)
        {
            return NotFound();
        }

        _context.Skills.Remove(skill);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SkillExists(int id)
    {
        return _context.Skills.Any(s => s.cod == id);
    }
    
}