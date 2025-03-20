using ESII2025d2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ESII2025d2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TalentoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TalentoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Talento
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Talento>>> GetTalento()
        {
            return await _context.Talentos.ToListAsync();
        }

        // GET: api/Talento/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Talento>> GetTalento(int id)
        {
            var talento = await _context.Talentos.FindAsync(id);

            if (talento == null)
            {
                return NotFound();
            }

            return talento;
        }

        // POST: api/Talento
        [HttpPost]
        public async Task<ActionResult<Talento>> CreateTalento(Talento novoTalento)
        {
            _context.Talentos.Add(novoTalento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTalento), new { id = novoTalento.id }, novoTalento);
        }

        // PUT: api/Talento/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTalento(int id, Talento talentoAtualizado)
        {
            if (id != talentoAtualizado.id)
            {
                return BadRequest("O ID do talento não corresponde.");
            }

            _context.Entry(talentoAtualizado).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TalentoExists(id))
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
        public async Task<IActionResult> DeleteTalento(int id)
        {
            var talento = await _context.Talentos
                .Include(t => t.Experiencia) // Incluir experiências associadas
                .Include(t => t.TalentoSkills) // Incluir talentos skills associadas
                .FirstOrDefaultAsync(t => t.id == id);

            if (talento == null)
            {
                return NotFound();
            }

            // Remover registros dependentes primeiro
            if (talento.Experiencia != null && talento.Experiencia.Any())
            {
                _context.Experiencias.RemoveRange(talento.Experiencia);
            }

            if (talento.TalentoSkills != null && talento.TalentoSkills.Any())
            {
                _context.TalentoSkills.RemoveRange(talento.TalentoSkills);
            }

            _context.Talentos.Remove(talento);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool TalentoExists(int id)
        {
            return _context.Talentos.Any(t => t.id == id);
        }
    }
}
