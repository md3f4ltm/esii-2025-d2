using ESII2025d2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ESII2025d2.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class TalentoSkillController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TalentoSkillController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TalentoSkill>>> GetTalentoSkill()
    {
        return await _context.TalentoSkills.ToListAsync();
    }
    
        [HttpGet("{idtalento}/{codskill}")]
    public async Task<ActionResult<TalentoSkill>> GetTalentoSkill(int idtalento, int codskill)
    {
        var talentoSkill = await _context.TalentoSkills
            .Include(ts => ts.idtalento) // Inclui o Talento associado
            .Include(ts => ts.codskill) // Inclui a Skill associada
            .FirstOrDefaultAsync(ts => ts.idtalento == idtalento && ts.codskill == codskill);

        if (talentoSkill == null)
        {
            return NotFound();
        }

        return talentoSkill;
    }

    // POST: api/TalentoSkill
    [HttpPost]
    public async Task<ActionResult<TalentoSkill>> CreateTalentoSkill(TalentoSkill novoTalentoSkill)
    {
        // Verifica se os IDs referenciados existem
        if (!await _context.Talentos.AnyAsync(t => t.id == novoTalentoSkill.idtalento))
            return BadRequest("Talento não encontrado.");

        if (!await _context.Skills.AnyAsync(s => s.cod == novoTalentoSkill.codskill))
            return BadRequest("Skill não encontrada.");

        _context.TalentoSkills.Add(novoTalentoSkill);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTalentoSkill), new { idtalento = novoTalentoSkill.idtalento, codskill = novoTalentoSkill.codskill }, novoTalentoSkill);
    }

    // PUT: api/TalentoSkill/5/10
    [HttpPut("{idtalento}/{codskill}")]
    public async Task<IActionResult> UpdateTalentoSkill(int idtalento, int codskill, TalentoSkill talentoSkillAtualizado)
    {
        if (idtalento != talentoSkillAtualizado.idtalento || codskill != talentoSkillAtualizado.codskill)
        {
            return BadRequest("Os IDs fornecidos não correspondem.");
        }

        if (!await _context.Talentos.AnyAsync(t => t.id == talentoSkillAtualizado.idtalento))
            return BadRequest("Talento não encontrado.");

        if (!await _context.Skills.AnyAsync(s => s.cod == talentoSkillAtualizado.codskill))
            return BadRequest("Skill não encontrada.");

        _context.Entry(talentoSkillAtualizado).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TalentoSkillExists(idtalento, codskill))
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

    // DELETE: api/TalentoSkill/5/10
    [HttpDelete("{idtalento}/{codskill}")]
    public async Task<IActionResult> DeleteTalentoSkill(int idtalento, int codskill)
    {
        var talentoSkill = await _context.TalentoSkills.FindAsync(idtalento, codskill);
        if (talentoSkill == null)
        {
            return NotFound();
        }

        _context.TalentoSkills.Remove(talentoSkill);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TalentoSkillExists(int idtalento, int codskill)
    {
        return _context.TalentoSkills.Any(ts => ts.idtalento == idtalento && ts.codskill == codskill);
    }
    
}