using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using esii_2025_d2.Data;
using esii_2025_d2.Models;

namespace esii_2025_d2.Services
{
    public interface IExperienceService
    {
        Task<List<Experience>> GetMyExperiencesAsync(string userId);
        Task<List<Experience>> GetAllExperiencesAsync();
        Task<Experience?> CreateExperienceAsync(Experience experience);
        Task<bool> UpdateExperienceAsync(Experience experience);
        Task<bool> DeleteExperienceAsync(int id, string userId);
    }

    public class ExperienceService : IExperienceService
    {
        private readonly ApplicationDbContext _context;

        public ExperienceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Experience>> GetMyExperiencesAsync(string userId)
        {
            return await _context.Experiences
                .Include(e => e.Talent)
                .Where(e => e.Talent != null && e.Talent.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Experience>> GetAllExperiencesAsync()
        {
            return await _context.Experiences
                .Include(e => e.Talent)
                .ToListAsync();
        }

        public async Task<Experience?> CreateExperienceAsync(Experience experience)
        {
            try
            {
                _context.Experiences.Add(experience);
                await _context.SaveChangesAsync();
                return experience;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateExperienceAsync(Experience experience)
        {
            try
            {
                _context.Entry(experience).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteExperienceAsync(int id, string userId)
        {
            try
            {
                var experience = await _context.Experiences
                    .Include(e => e.Talent)
                    .FirstOrDefaultAsync(e => e.Id == id && e.Talent != null && e.Talent.UserId == userId);
                
                if (experience == null) return false;

                _context.Experiences.Remove(experience);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
