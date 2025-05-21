using esii_2025_d2.Models;
using esii_2025_d2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace esii_2025_d2.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private const int STANDARD_MONTHLY_HOURS = 176;

    public ReportsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/reports/bycategorycountry
    [HttpGet("bycategorycountry")]
    [Authorize(Roles = "Customer,Admin")]
    public async Task<ActionResult<IEnumerable<CategoryCountryReportItem>>> GetCategoryCountryReport()
    {
        var report = await _context.Talents
            .Where(t => t.IsPublic)
            .GroupBy(t => new { CategoryId = t.TalentCategoryId, t.Country })
            .Select(g => new CategoryCountryReportItem
            {
                CategoryId = g.Key.CategoryId,
                Country = g.Key.Country,
                AverageHourlyRate = g.Average(t => t.HourlyRate),
                TalentCount = g.Count()
            })
            .ToListAsync();

        // Load category names
        var categories = await _context.TalentCategories.ToDictionaryAsync(c => c.Id, c => c.Name);

        foreach (var item in report)
        {
            // Set category name
            if (categories.TryGetValue(item.CategoryId, out string? categoryName))
            {
                item.CategoryName = categoryName;
            }
            else
            {
                item.CategoryName = "Unknown Category";
            }

            // Calculate monthly rate
            item.AverageMonthlyRate = item.AverageHourlyRate * STANDARD_MONTHLY_HOURS;
        }

        return report;
    }

    // GET: api/reports/byskill
    [HttpGet("byskill")]
    [Authorize(Roles = "Customer,Admin")]
    public async Task<ActionResult<IEnumerable<SkillReportItem>>> GetSkillReport()
    {
        var talentSkills = await _context.TalentSkills
            .Include(ts => ts.Talent)
            .Include(ts => ts.Skill)
            .Where(ts => ts.Talent.IsPublic)
            .ToListAsync();

        // Group by skill
        var report = talentSkills
            .GroupBy(ts => new { SkillId = ts.SkillId })
            .Select(g => new SkillReportItem
            {
                SkillId = g.Key.SkillId,
                SkillName = g.First().Skill.Name,
                Area = g.First().Skill.Area,
                AverageHourlyRate = g.Average(ts => ts.Talent.HourlyRate),
                TalentCount = g.Count()
            })
            .ToList();

        // Calculate monthly rate
        foreach (var item in report)
        {
            item.AverageMonthlyRate = item.AverageHourlyRate * STANDARD_MONTHLY_HOURS;
        }

        return report;
    }

    public class CategoryCountryReportItem
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public decimal AverageHourlyRate { get; set; }
        public decimal AverageMonthlyRate { get; set; }
        public int TalentCount { get; set; }
    }

    public class SkillReportItem
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string? Area { get; set; }
        public decimal AverageHourlyRate { get; set; }
        public decimal AverageMonthlyRate { get; set; }
        public int TalentCount { get; set; }
    }
}