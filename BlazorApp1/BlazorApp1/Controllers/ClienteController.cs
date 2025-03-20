using ESII2025d2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ESII2025d2.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ClienteController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ClienteController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetCliente()
    {
        return await _context.Clientes.ToListAsync();
    }
    
// GET: api/Cliente/5
[HttpGet("{id}")]
public async Task<ActionResult<Cliente>> GetCliente(int id)
{
    var cliente = await _context.Clientes
        .Include(c => c.PropostaTrabalhos)  // Incluindo Propostas de Trabalho associadas
        .Include(c => c.idutilizadorNavigation)  // Incluindo o Utilizador associado
        .SingleOrDefaultAsync(c => c.id == id);

    if (cliente == null)
    {
        return NotFound();
    }

    return cliente;
}

// POST: api/Cliente
[HttpPost]
public async Task<ActionResult<Cliente>> CreateCliente(Cliente novoCliente)
{
    _context.Clientes.Add(novoCliente);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetCliente), new { id = novoCliente.id }, novoCliente);
}

// PUT: api/Cliente/5
[HttpPut("{id}")]
public async Task<IActionResult> UpdateCliente(int id, Cliente clienteAtualizado)
{
    if (id != clienteAtualizado.id)
    {
        return BadRequest("O ID do cliente não corresponde.");
    }

    _context.Entry(clienteAtualizado).State = EntityState.Modified;

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!ClienteExists(id))
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

// DELETE: api/Cliente/5
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteCliente(int id)
{
    var cliente = await _context.Clientes
        .Include(c => c.PropostaTrabalhos)  // Incluindo propostas de trabalho associadas
        .Include(c => c.idutilizadorNavigation)  // Incluindo o utilizador associado
        .FirstOrDefaultAsync(c => c.id == id);

    if (cliente == null)
    {
        return NotFound();
    }

    // Remover registros dependentes primeiro, se necessário
    if (cliente.PropostaTrabalhos != null && cliente.PropostaTrabalhos.Any())
    {
        _context.PropostaTrabalhos.RemoveRange(cliente.PropostaTrabalhos);
    }

    _context.Clientes.Remove(cliente);
    await _context.SaveChangesAsync();

    return NoContent();
}

private bool ClienteExists(int id)
{
    return _context.Clientes.Any(c => c.id == id);
}

}