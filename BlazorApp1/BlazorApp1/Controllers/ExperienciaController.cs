using ESII2025d2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ESII2025d2.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ExperienciaController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ExperienciaController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Experiencia>>> GetExperiencia()
    {
        return await _context.Experiencias.ToListAsync();
    }
    
        // GET: api/Experiencia/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Experiencia>> GetExperiencia(int id)
    {
        var experiencia = await _context.Experiencias
            .Include(e => e.idtalento)  // Incluindo Talento associado
            .SingleOrDefaultAsync(e => e.id == id);

        if (experiencia == null)
        {
            return NotFound();
        }

        return experiencia;
    }

    // POST: api/Experiencia
    [HttpPost]
    public async Task<ActionResult<Experiencia>> CreateExperiencia(Experiencia novaExperiencia)
    {
        // Verificar se o Talento associado existe
        var talento = await _context.Talentos.FindAsync(novaExperiencia.id);
        if (talento == null)
        {
            return NotFound("Talento não encontrado.");
        }

        _context.Experiencias.Add(novaExperiencia);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExperiencia), new { id = novaExperiencia.id }, novaExperiencia);
    }

    // PUT: api/Experiencia/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExperiencia(int id, Experiencia experienciaAtualizada)
    {
        if (id != experienciaAtualizada.id)
        {
            return BadRequest("O ID da experiência não corresponde.");
        }

        // Verificar se o Talento associado existe
        var talento = await _context.Talentos.FindAsync(experienciaAtualizada.id);
        if (talento == null)
        {
            return NotFound("Talento não encontrado.");
        }

        _context.Entry(experienciaAtualizada).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ExperienciaExists(id))
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

    // DELETE: api/Experiencia/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExperiencia(int id)
    {
        var experiencia = await _context.Experiencias
            .Include(e => e.idtalento)  // Incluindo Talento associado
            .FirstOrDefaultAsync(e => e.id == id);

        if (experiencia == null)
        {
            return NotFound();
        }

        _context.Experiencias.Remove(experiencia);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ExperienciaExists(int id)
    {
        return _context.Experiencias.Any(e => e.id == id);
    }
    
}