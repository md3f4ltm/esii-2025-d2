using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using esii_2025_d2.Data;
using esii_2025_d2.Models;
using esii_2025_d2.DTOs;

namespace esii_2025_d2.Services
{
    public interface ITalentSkillService
    {
        Task<List<TalentSkillDto>> GetTalentSkillsAsync(int talentId);
        Task<TalentSkillDto> CreateTalentSkillAsync(TalentSkillDto talentSkill);
        Task<TalentSkillDto> UpdateTalentSkillAsync(TalentSkillDto talentSkill);
        Task DeleteTalentSkillAsync(int talentId, int skillId);
    }

    public class TalentSkillService : ITalentSkillService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthenticationStateProvider _authStateProvider;

        public TalentSkillService(ApplicationDbContext context, AuthenticationStateProvider authStateProvider)
        {
            _context = context;
            _authStateProvider = authStateProvider;
        }

        private async Task<string> GetCurrentUserIdAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var userId = authState.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User not authenticated or user ID not found.");
            }

            return userId;
        }

        public async Task<List<TalentSkillDto>> GetTalentSkillsAsync(int talentId)
        {
            var userId = await GetCurrentUserIdAsync();

            // Check if talent exists and belongs to the current user
            var talent = await _context.Talents.FirstOrDefaultAsync(t => t.Id == talentId && t.UserId == userId);
            if (talent == null)
            {
                throw new UnauthorizedAccessException("Talent not found or access denied.");
            }

            // Get all skills for this talent
            var talentSkills = await _context.TalentSkills
                .Where(ts => ts.TalentId == talentId)
                .Include(ts => ts.Skill)
                .Select(ts => new TalentSkillDto
                {
                    TalentId = ts.TalentId,
                    SkillId = ts.SkillId,
                    YearsOfExperience = ts.YearsOfExperience,
                    Skill = ts.Skill
                })
                .ToListAsync();

            return talentSkills;
        }

        public async Task<TalentSkillDto> CreateTalentSkillAsync(TalentSkillDto talentSkill)
        {
            var userId = await GetCurrentUserIdAsync();

            // Check if talent exists and belongs to the current user
            var talent = await _context.Talents.FirstOrDefaultAsync(t => t.Id == talentSkill.TalentId && t.UserId == userId);
            if (talent == null)
            {
                throw new UnauthorizedAccessException($"Talent with ID {talentSkill.TalentId} not found or does not belong to the current user.");
            }

            // Check if skill exists
            if (!await _context.Skills.AnyAsync(s => s.Id == talentSkill.SkillId))
            {
                throw new InvalidOperationException($"Skill with ID {talentSkill.SkillId} not found.");
            }

            // Check if the combination already exists
            if (await _context.TalentSkills.AnyAsync(ts => ts.TalentId == talentSkill.TalentId && ts.SkillId == talentSkill.SkillId))
            {
                throw new InvalidOperationException("This talent already has this skill assigned.");
            }

            var newTalentSkill = new TalentSkill
            {
                TalentId = talentSkill.TalentId,
                SkillId = talentSkill.SkillId,
                YearsOfExperience = talentSkill.YearsOfExperience
            };

            _context.TalentSkills.Add(newTalentSkill);
            await _context.SaveChangesAsync();

            // Load the skill details
            var skill = await _context.Skills.FindAsync(talentSkill.SkillId);
            talentSkill.Skill = skill;

            return talentSkill;
        }

        public async Task<TalentSkillDto> UpdateTalentSkillAsync(TalentSkillDto talentSkill)
        {
            var userId = await GetCurrentUserIdAsync();

            // Check if talent exists and belongs to the current user
            var talent = await _context.Talents.FirstOrDefaultAsync(t => t.Id == talentSkill.TalentId && t.UserId == userId);
            if (talent == null)
            {
                throw new UnauthorizedAccessException($"Talent with ID {talentSkill.TalentId} not found or does not belong to the current user.");
            }

            var existingTalentSkill = await _context.TalentSkills.FindAsync(talentSkill.TalentId, talentSkill.SkillId);
            if (existingTalentSkill == null)
            {
                throw new InvalidOperationException($"Talent skill not found.");
            }

            existingTalentSkill.YearsOfExperience = talentSkill.YearsOfExperience;
            await _context.SaveChangesAsync();

            // Load the skill details
            var skill = await _context.Skills.FindAsync(talentSkill.SkillId);
            talentSkill.Skill = skill;

            return talentSkill;
        }

        public async Task DeleteTalentSkillAsync(int talentId, int skillId)
        {
            var userId = await GetCurrentUserIdAsync();

            // Check if talent exists and belongs to the current user
            var talent = await _context.Talents.FirstOrDefaultAsync(t => t.Id == talentId && t.UserId == userId);
            if (talent == null)
            {
                throw new UnauthorizedAccessException("Talent not found or access denied.");
            }

            var talentSkill = await _context.TalentSkills.FindAsync(talentId, skillId);
            if (talentSkill == null)
            {
                throw new InvalidOperationException($"Talent skill not found.");
            }

            _context.TalentSkills.Remove(talentSkill);
            await _context.SaveChangesAsync();
        }
    }
}
