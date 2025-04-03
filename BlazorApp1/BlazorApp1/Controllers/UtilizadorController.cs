using System.Security.Cryptography;
using System.Text;
using ESII2025d2.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
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
    public async Task<ActionResult<Utilizador>> CreateUtilizador([FromBody] Utilizador novoUtilizador)
    {
        if (await _context.Utilizadores.AnyAsync(u => u.email == novoUtilizador.email))
        {
            return Conflict("Já existe um utilizador com este e-mail.");
        }

        // Encriptar a password antes de guardar
        novoUtilizador.palavra_passe = HashPassword(novoUtilizador.palavra_passe);

        _context.Utilizadores.Add(novoUtilizador);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUtilizador), new { id = novoUtilizador.id }, novoUtilizador);
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginRequest request)
    {
        var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(u => u.email == request.Email);

        if (utilizador == null || utilizador.palavra_passe != HashPassword(request.Password))
        {
            return Unauthorized("E-mail ou password incorretos.");
        }

        return Ok($"Login bem-sucedido! Bem-vindo, {utilizador.nome}");
    }

// Método para encriptar a password (SHA-256)
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
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

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}