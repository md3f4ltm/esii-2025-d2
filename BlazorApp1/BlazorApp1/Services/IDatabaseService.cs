using BlazorApp1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp1.Services
{
    public interface IDatabaseService
    {
        // ApplicationUser methods
        Task<List<ApplicationUser>> GetUtilizadores();
        Task<ApplicationUser?> GetUtilizadorById(string id);
        Task<ApplicationUser> CreateUtilizador(ApplicationUser utilizador);
        Task<bool> UpdateUtilizador(ApplicationUser utilizador);
        Task<bool> DeleteUtilizador(string id);

        // Cliente methods
        Task<List<Cliente>> GetClientes();
        Task<Cliente?> GetClienteById(int id);
        Task<Cliente> CreateCliente(Cliente cliente);

        // CategoriaTalento methods
        Task<List<CategoriaTalento>> GetCategoriasTalento();
        Task<CategoriaTalento?> GetCategoriaTalentoById(int id);

        // Talento methods
        Task<List<Talento>> GetTalentos();
        Task<Talento?> GetTalentoById(int id);
        Task<List<Talento>> GetTalentosByCategoria(int categoriaId);

        // Skill methods
        Task<List<Skill>> GetSkills();
        Task<List<Skill>> GetSkillsForTalento(int talentoId);

        // PropostaTrabalho methods
        Task<List<PropostaTrabalho>> GetPropostasTrabalho();
        Task<PropostaTrabalho?> GetPropostaTrabalhoById(int id);
        Task<PropostaTrabalho> CreatePropostaTrabalho(PropostaTrabalho proposta);
    }
}
