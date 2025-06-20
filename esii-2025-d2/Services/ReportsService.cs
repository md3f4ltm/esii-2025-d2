using Microsoft.EntityFrameworkCore;
using esii_2025_d2.Data;
using esii_2025_d2.Models;

namespace esii_2025_d2.Services
{
    public interface IReportsService
    {
        Task<List<CategoryCountryReport>> GetCategoryCountryReportAsync();
        Task<List<SkillReport>> GetSkillReportAsync();
    }

    public class ReportsService : IReportsService
    {
        private readonly ApplicationDbContext _context;
        private const int STANDARD_MONTHLY_HOURS = 176; // Standard for monthly rate calculation

        public ReportsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryCountryReport>> GetCategoryCountryReportAsync()
        {
            var report = await _context.Talents
                .Include(t => t.TalentCategory)
                .Where(t => t.Country != null && t.HourlyRate > 0 && t.TalentCategoryId != null)
                .GroupBy(t => new { t.TalentCategory!.Name, t.Country })
                .Select(g => new CategoryCountryReport
                {
                    CategoryName = g.Key.Name,
                    Country = g.Key.Country!,
                    AverageMonthlyRate = g.Average(t => t.HourlyRate * STANDARD_MONTHLY_HOURS),
                    TalentCount = g.Count()
                })
                .ToListAsync();

            return report;
        }

        public async Task<List<SkillReport>> GetSkillReportAsync()
        {
            var report = await _context.TalentSkills
                .Include(ts => ts.Skill)
                .Include(ts => ts.Talent)
                .Where(ts => ts.Talent.HourlyRate > 0)
                .GroupBy(ts => new { ts.Skill.Name, ts.Skill.Area })
                .Select(g => new SkillReport
                {
                    SkillName = g.Key.Name,
                    Area = g.Key.Area,
                    AverageMonthlyRate = g.Average(ts => ts.Talent.HourlyRate * STANDARD_MONTHLY_HOURS),
                    TalentCount = g.Select(ts => ts.TalentId).Distinct().Count()
                })
                .ToListAsync();

            return report;
        }
    }
}
