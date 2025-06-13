using Microsoft.EntityFrameworkCore;
using esii_2025_d2.Data;
using esii_2025_d2.Models;

namespace esii_2025_d2.Services
{
    public class SkillService : ISkillService
    {
        private readonly ApplicationDbContext _context;

        public SkillService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Skill>> GetAllSkillsAsync()
        {
            return await _context.Skills
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Skill?> GetSkillByIdAsync(int id)
        {
            return await _context.Skills
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Skill> CreateSkillAsync(Skill skill)
        {
            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();
            return skill;
        }

        public async Task<bool> UpdateSkillAsync(Skill skill)
        {
            var existingSkill = await _context.Skills
                .FirstOrDefaultAsync(s => s.Id == skill.Id);

            if (existingSkill == null)
                return false;

            _context.Entry(existingSkill).CurrentValues.SetValues(skill);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSkillAsync(int id)
        {
            var skill = await _context.Skills
                .FirstOrDefaultAsync(s => s.Id == id);

            if (skill == null)
                return false;

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
