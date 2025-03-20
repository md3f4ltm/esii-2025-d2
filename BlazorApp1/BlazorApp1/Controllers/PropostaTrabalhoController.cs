using ESII2025d2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ESII2025d2.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class PropostaTrabalhoController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PropostaTrabalhoController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PropostaTrabalho>>> GetPropostaTrabalho()
    {
        return await _context.PropostaTrabalhos.ToListAsync();
    }
    
        [HttpGet("{id}")]
    public async Task<ActionResult<PropostaTrabalho>> GetPropostaTrabalho(int id)
    {
        var proposta = await _context.PropostaTrabalhos
            .Include(p => p.cliente_id) // Inclui o cliente associado
            .Include(p => p.cattalento_cod) // Inclui a categoria do talento
            .Include(p => p.codskill) // Inclui a skill associada
            .FirstOrDefaultAsync(p => p.cod == id);

        if (proposta == null)
        {
            return NotFound();
        }

        return proposta;
    }

    // POST: api/PropostaTrabalho
    [HttpPost]
    public async Task<ActionResult<PropostaTrabalho>> CreatePropostaTrabalho(PropostaTrabalho novaProposta)
    {
        // Verifica se os IDs referenciados existem
        if (!await _context.Clientes.AnyAsync(c => c.id == novaProposta.cliente_id))
            return BadRequest("Cliente não encontrado.");

        if (!await _context.CategoriasTalento.AnyAsync(ct => ct.cod == novaProposta.cattalento_cod))
            return BadRequest("Categoria de Talento não encontrada.");

        if (!await _context.Skills.AnyAsync(s => s.cod == novaProposta.codskill))
            return BadRequest("Skill não encontrada.");

        _context.PropostaTrabalhos.Add(novaProposta);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPropostaTrabalho), new { id = novaProposta.cod}, novaProposta);
    }

    // PUT: api/PropostaTrabalho/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePropostaTrabalho(int id, PropostaTrabalho propostaAtualizada)
    {
        if (id != propostaAtualizada.cod)
        {
            return BadRequest("O ID da proposta não corresponde.");
        }

        if (!await _context.Clientes.AnyAsync(c => c.id == propostaAtualizada.cliente_id))
            return BadRequest("Cliente não encontrado.");

        if (!await _context.CategoriasTalento.AnyAsync(ct => ct.cod == propostaAtualizada.cattalento_cod))
            return BadRequest("Categoria de Talento não encontrada.");

        if (!await _context.Skills.AnyAsync(s => s.cod == propostaAtualizada.codskill))
            return BadRequest("Skill não encontrada.");

        _context.Entry(propostaAtualizada).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PropostaTrabalhoExists(id))
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

    // DELETE: api/PropostaTrabalho/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePropostaTrabalho(int id)
    {
        var proposta = await _context.PropostaTrabalhos.FindAsync(id);
        if (proposta == null)
        {
            return NotFound();
        }

        _context.PropostaTrabalhos.Remove(proposta);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PropostaTrabalhoExists(int id)
    {
        return _context.PropostaTrabalhos.Any(p => p.cod == id);
    }
    
}