using esii_2025_d2.Models;
using esii_2025_d2.Data;
using esii_2025_d2.Services;
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
    private readonly IReportsService _reportsService;
    private const int STANDARD_MONTHLY_HOURS = 176;

    public ReportsController(ApplicationDbContext context, IReportsService reportsService)
    {
        _context = context;
        _reportsService = reportsService;
    }

    // GET: api/reports/bycategorycountry
    [HttpGet("bycategorycountry")]
    [Authorize(Roles = "Customer,Admin")]
    public async Task<ActionResult<IEnumerable<CategoryCountryReport>>> GetCategoryCountryReport()
    {
        try
        {
            var report = await _reportsService.GetCategoryCountryReportAsync();
            return Ok(report);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // GET: api/reports/byskill
    [HttpGet("byskill")]
    [Authorize(Roles = "Customer,Admin")]
    public async Task<ActionResult<IEnumerable<SkillReport>>> GetSkillReport()
    {
        try
        {
            var report = await _reportsService.GetSkillReportAsync();
            return Ok(report);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

}