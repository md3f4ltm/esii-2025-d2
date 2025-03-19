using BlazorApp1.Data;
using BlazorApp1.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly ApplicationDbContext _db;

        public DatabaseService(ApplicationDbContext db)
        {
            _db = db;
        }

        // ApplicationUser methods
        public async Task<List<ApplicationUser>> GetUtilizadores()
        {
            return await _db.Users.ToListAsync();
        }

        public async Task<ApplicationUser?> GetUtilizadorById(string id)
        {
            return await _db.Users.FindAsync(id);
        }

        public async Task<ApplicationUser> CreateUtilizador(ApplicationUser utilizador)
        {
            _db.Users.Add(utilizador);
            await _db.SaveChangesAsync();
            return utilizador;
        }

        public async Task<bool> UpdateUtilizador(ApplicationUser utilizador)
        {
            _db.Users.Update(utilizador);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteUtilizador(string id)
        {
            var utilizador = await _db.Users.FindAsync(id);
            if (utilizador == null) return false;

            _db.Users.Remove(utilizador);
            return await _db.SaveChangesAsync() > 0;
        }

        // Cliente methods
        public async Task<List<Cliente>> GetClientes()
        {
            return await _db.Clientes
                .Include(c => c.User)
                .ToListAsync();
        }

        public async Task<Cliente?> GetClienteById(int id)
        {
            return await _db.Clientes
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cliente> CreateCliente(Cliente cliente)
        {
            _db.Clientes.Add(cliente);
            await _db.SaveChangesAsync();
            return cliente;
        }

        // CategoriaTalento methods
        public async Task<List<CategoriaTalento>> GetCategoriasTalento()
        {
            return await _db.CategoriasTalento.ToListAsync();
        }

        public async Task<CategoriaTalento?> GetCategoriaTalentoById(int id)
        {
            return await _db.CategoriasTalento.FindAsync(id);
        }

        // Talento methods
        public async Task<List<Talento>> GetTalentos()
        {
            return await _db.Talentos
                .Include(t => t.CategoriaTalento)
                .Include(t => t.User)
                .ToListAsync();
        }

        public async Task<Talento?> GetTalentoById(int id)
        {
            return await _db.Talentos
                .Include(t => t.CategoriaTalento)
                .Include(t => t.User)
                .Include(t => t.Experiencias)
                .Include(t => t.TalentoSkills)
                    .ThenInclude(ts => ts.Skill)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Talento>> GetTalentosByCategoria(int categoriaId)
        {
            return await _db.Talentos
                .Where(t => t.CodCategoriaTalento == categoriaId)
                .ToListAsync();
        }

        // Skill methods
        public async Task<List<Skill>> GetSkills()
        {
            var skillsList = await _db.Skills.ToListAsync();
            // Fix nullability mismatch
            return skillsList.Where(s => s != null).ToList()!;
        }

        public async Task<List<Skill>> GetSkillsForTalento(int talentoId)
        {
            return await _db.TalentoSkills
                .Where(ts => ts.IdTalento == talentoId)
                .Select(ts => ts.Skill)
                .ToListAsync();
        }

        // PropostaTrabalho methods
        public async Task<List<PropostaTrabalho>> GetPropostasTrabalho()
        {
            return await _db.PropostasTrabalho
                .Include(p => p.Skill)
                .Include(p => p.Talento)
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task<PropostaTrabalho?> GetPropostaTrabalhoById(int id)
        {
            return await _db.PropostasTrabalho
                .Include(p => p.Skill)
                .Include(p => p.Talento)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Cod == id);
        }

        public async Task<PropostaTrabalho> CreatePropostaTrabalho(PropostaTrabalho proposta)
        {
            _db.PropostasTrabalho.Add(proposta);
            await _db.SaveChangesAsync();
            return proposta;
        }
    }
}
