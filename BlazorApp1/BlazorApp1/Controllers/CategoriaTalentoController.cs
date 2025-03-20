using ESII2025d2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ESII2025d2.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class CategoriaTalentoController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CategoriaTalentoController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaTalento>>> GetCategoriaTalento()
    {
        return await _context.CategoriasTalento.ToListAsync();
    }
    
        [HttpPost]
    public async Task<ActionResult<CategoriaTalento>> CreateCategoriaTalento(CategoriaTalento novaCategoria)
    {
        // Verificar se já existe uma categoria com o mesmo nome
        if (await _context.CategoriasTalento.AnyAsync(c => c.nome == novaCategoria.nome))
        {
            return Conflict("Já existe uma categoria de talento com este nome.");
        }

        _context.CategoriasTalento.Add(novaCategoria);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCategoriaTalento), new { id = novaCategoria.cod }, novaCategoria);
    }

    // GET: api/CategoriaTalento/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoriaTalento>> GetCategoriaTalento(int id)
    {
        var categoriaTalento = await _context.CategoriasTalento.FindAsync(id);

        if (categoriaTalento == null)
        {
            return NotFound();
        }

        return categoriaTalento;
    }

    // PUT: api/CategoriaTalento/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategoriaTalento(int id, CategoriaTalento categoriaAtualizada)
    {
        if (id != categoriaAtualizada.cod)
        {
            return BadRequest("O ID da categoria de talento não corresponde.");
        }

        _context.Entry(categoriaAtualizada).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CategoriaTalentoExists(id))
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

    // DELETE: api/CategoriaTalento/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategoriaTalento(int id)
    {
        var categoriaTalento = await _context.CategoriasTalento.FindAsync(id);
        if (categoriaTalento == null)
        {
            return NotFound();
        }

        _context.CategoriasTalento.Remove(categoriaTalento);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CategoriaTalentoExists(int id)
    {
        return _context.CategoriasTalento.Any(c => c.cod == id);
    }
    
}