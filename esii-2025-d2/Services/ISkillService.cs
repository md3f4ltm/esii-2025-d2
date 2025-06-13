using esii_2025_d2.Models;

namespace esii_2025_d2.Services
{
    public interface ISkillService
    {
        Task<List<Skill>> GetAllSkillsAsync();
        Task<Skill?> GetSkillByIdAsync(int id);
        Task<Skill> CreateSkillAsync(Skill skill);
        Task<bool> UpdateSkillAsync(Skill skill);
        Task<bool> DeleteSkillAsync(int id);
    }
}
