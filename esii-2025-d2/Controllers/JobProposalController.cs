// esii-2025-d2/Controllers/JobProposalController.cs
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
public class JobProposalController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public JobProposalController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/JobProposal
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobProposal>>> GetJobProposals()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User not authenticated." });
        }

        // If user is a customer, return only their proposals
        // Otherwise, return all proposals (for admins, etc.)
        if (User.IsInRole("Customer"))
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
            if (customer == null)
            {
                return NotFound(new { message = "Customer profile not found for current user." });
            }

            return await _context.JobProposals
                .Where(jp => jp.CustomerId == customer.Id)
                .Include(jp => jp.Skill)
                .Include(jp => jp.TalentCategory)
                .Include(jp => jp.Customer)
                .ToListAsync();
        }
        else
        {
            return await _context.JobProposals
                .Include(jp => jp.Skill)
                .Include(jp => jp.TalentCategory)
                .Include(jp => jp.Customer)
                .ToListAsync();
        }
    }

    // GET: api/JobProposal/GetAllProposals
    [HttpGet("GetAllProposals")]
    public async Task<ActionResult<IEnumerable<JobProposal>>> GetAllProposals()
    {
        return await _context.JobProposals
            .Include(jp => jp.Skill)
            .Include(jp => jp.TalentCategory)
            .Include(jp => jp.Customer)
            .ToListAsync();
    }

    // GET: api/JobProposal/{id}/eligibletalents
    [HttpGet("{id}/eligibletalents")]
    public async Task<ActionResult<IEnumerable<object>>> GetEligibleTalents(int id)
    {
        var jobProposal = await _context.JobProposals
            .Include(jp => jp.Skill)
            .Include(jp => jp.TalentCategory)
            .FirstOrDefaultAsync(jp => jp.Id == id);

        if (jobProposal == null)
        {
            return NotFound(new { message = $"Job proposal with ID {id} not found." });
        }

        // Find talents with the required skill and category (if specified)
        var query = _context.Talents.Where(t => t.IsPublic);

        // Filter by talent category if specified
        if (jobProposal.TalentCategoryId.HasValue)
        {
            query = query.Where(t => t.TalentCategoryId == jobProposal.TalentCategoryId.Value);
        }

        // Filter talents who have the required skill
        var eligibleTalents = await query
            .Include(t => t.TalentSkills)
                .ThenInclude(ts => ts.Skill)
            .Include(t => t.TalentCategory)
            .Where(t => t.TalentSkills.Any(ts => ts.SkillId == jobProposal.SkillId))
            .ToListAsync();

        // Create a list of talents with calculated total cost
        var results = eligibleTalents.Select(talent => new {
            Id = talent.Id,
            Name = talent.Name,
            Country = talent.Country,
            Email = talent.Email,
            HourlyRate = talent.HourlyRate,
            TotalCost = talent.HourlyRate * jobProposal.TotalHours,
            TalentCategory = talent.TalentCategory != null ? new { talent.TalentCategory.Id, talent.TalentCategory.Name } : null
        })
        .OrderBy(t => t.Name)
        .ToList();

        return results;
    }

    // GET: api/JobProposal/5
    [HttpGet("{id}")]
    public async Task<ActionResult<JobProposal>> GetJobProposal(int id)
    {
        var jobProposal = await _context.JobProposals
            .Include(jp => jp.Skill)
            .Include(jp => jp.TalentCategory)
            .Include(jp => jp.Customer)
            .FirstOrDefaultAsync(jp => jp.Id == id);

        if (jobProposal == null)
        {
            return NotFound();
        }

        return jobProposal;
    }

    // PUT: api/JobProposal/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutJobProposal(int id, JobProposal jobProposal)
    {
        if (id != jobProposal.Id)
        {
            return BadRequest();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User not authenticated." });
        }

        // Verify the job proposal exists and belongs to the user's customer
        var existingProposal = await _context.JobProposals
            .Include(jp => jp.Customer)
            .FirstOrDefaultAsync(jp => jp.Id == id);
            
        if (existingProposal == null)
        {
            return NotFound();
        }
        
        if (existingProposal.Customer?.UserId != userId)
        {
            return Forbid();
        }

        // Ensure the customer ID doesn't change or is valid
        if (jobProposal.CustomerId != existingProposal.CustomerId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == jobProposal.CustomerId);
            if (customer == null || customer.UserId != userId)
            {
                return BadRequest(new { message = "Invalid or unauthorized customer ID." });
            }
        }

        _context.Entry(jobProposal).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!JobProposalExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/JobProposal
    [HttpPost]
    public async Task<ActionResult<JobProposal>> PostJobProposal(JobProposal jobProposal)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User not authenticated." });
        }

        // Verify that the customer belongs to the current user
        if (!string.IsNullOrEmpty(jobProposal.CustomerId))
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == jobProposal.CustomerId);
            if (customer == null || customer.UserId != userId)
            {
                return BadRequest(new { message = "Invalid or unauthorized customer ID." });
            }
        }
        else
        {
            // If no customer ID provided, try to find the customer for the current user
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
            if (customer == null)
            {
                return BadRequest(new { message = "Customer profile not found. Please create a customer profile first." });
            }

            jobProposal.CustomerId = customer.Id;
        }

        _context.JobProposals.Add(jobProposal);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetJobProposal), new { id = jobProposal.Id }, jobProposal);
    }

    // DELETE: api/JobProposal/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJobProposal(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User not authenticated." });
        }

        var jobProposal = await _context.JobProposals
            .Include(jp => jp.Customer)
            .FirstOrDefaultAsync(jp => jp.Id == id);
            
        if (jobProposal == null)
        {
            return NotFound();
        }

        // Verify that the job proposal belongs to the user's customer
        if (jobProposal.Customer?.UserId != userId)
        {
            return Forbid();
        }

        _context.JobProposals.Remove(jobProposal);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool JobProposalExists(int id)
    {
        return _context.JobProposals.Any(e => e.Id == id);
    }
}