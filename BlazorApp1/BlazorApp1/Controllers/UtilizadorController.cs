using Microsoft.AspNetCore.Identity;
using System.Text;
using ESII2025d2.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ESII2025d2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilizadorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<Utilizador> _passwordHasher;

        public UtilizadorController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Utilizador>();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Utilizador>>> GetUtilizador()
        {
            return await _context.Utilizadores.ToListAsync();
        }

        [HttpPost]
        [Route("api/utilizadores")]
        public async Task<IActionResult> CriarUtilizador([FromBody] Utilizador novoUtilizador)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var passwordHasher = new PasswordHasher<Utilizador>();
            novoUtilizador.Password = passwordHasher.HashPassword(novoUtilizador, novoUtilizador.Password);

            _context.Utilizadores.Add(novoUtilizador);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Conta criada com sucesso!" });
        }



        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequest request)
        {
            var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (utilizador == null)
            {
                return Unauthorized("E-mail ou password incorretos.");
            }

            // Verificar a senha fornecida com o hash armazenado
            var result = _passwordHasher.VerifyHashedPassword(utilizador, utilizador.Password, request.Password);

            if (result != PasswordVerificationResult.Success)
            {
                return Unauthorized("E-mail ou password incorretos.");
            }

            return Ok($"Login bem-sucedido! Bem-vindo, {utilizador.Nome}");
        }


        private bool UtilizadorExists(int id)
        {
            return _context.Utilizadores.Any(u => u.Id == id);
        }

        // PUT: api/Utilizador/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUtilizador(int id, Utilizador utilizadorAtualizado)
        {
            if (id != utilizadorAtualizado.Id)
            {
                return BadRequest("O ID do utilizador não corresponde.");
            }

            var talento = await _context.Talentos.FirstOrDefaultAsync(t => t.idutilizador == id);
            if (talento != null)
            {
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
}

