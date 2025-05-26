using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using esii_2025_d2.Data;
using esii_2025_d2.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace esii_2025_d2.Services
{
    public interface ITalentService
    {
        Task<List<Talent>> GetMyTalentsAsync(string userId);
        Task<List<Talent>> GetAllTalentsAsync();
    }

    public class TalentService : ITalentService
    {
        private readonly ApplicationDbContext _context;

        public TalentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Talent>> GetMyTalentsAsync(string userId)
        {
            return await _context.Talents
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Talent>> GetAllTalentsAsync()
        {
            return await _context.Talents.ToListAsync();
        }
    }
}
