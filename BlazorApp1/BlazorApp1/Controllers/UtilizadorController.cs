using ESII2025d2.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ESII2025d2.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class UtilizadorController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UtilizadorController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Utilizador>>> GetUtilizador()
    {
        return await _context.Utilizadores.ToListAsync();
    }
    
    [HttpPost]
    public async Task<ActionResult<Utilizador>> CreateUtilizador(Utilizador novoUtilizador)
    {
        if (await _context.Utilizadores.AnyAsync(u => u.email == novoUtilizador.email))
        {
            return Conflict("Já existe um utilizador com este e-mail.");
        }

        _context.Utilizadores.Add(novoUtilizador);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUtilizador), new { id = novoUtilizador.id }, novoUtilizador);
    }
    
    private bool UtilizadorExists(int id)
    {
        return _context.Utilizadores.Any(u => u.id == id);
    }

    
    // PUT: api/Utilizador/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUtilizador(int id, Utilizador utilizadorAtualizado)
    {
        if (id != utilizadorAtualizado.id)
        {
            return BadRequest("O ID do utilizador não corresponde.");
        }
        
        var talento = await _context.Talentos.FirstOrDefaultAsync(t => t.idutilizador == id);
        if (talento != null)
        {
            // Atualizar os dados no talento também
            talento.nome = utilizadorAtualizado.nome;
            talento.email = utilizadorAtualizado.email;
            _context.Entry(talento).State = EntityState.Modified;
        }

        _context.Entry(utilizadorAtualizado).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UtilizadorExists(id))
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
    
    // DELETE: api/Utilizador/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUtilizador(int id)
    {
        var utilizador = await _context.Utilizadores.FindAsync(id);
        if (utilizador == null)
        {
            return NotFound();
        }

        _context.Utilizadores.Remove(utilizador);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
}