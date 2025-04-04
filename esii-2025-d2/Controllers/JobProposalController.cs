// esii-2025-d2/Controllers/JobProposalController.cs
using esii_2025_d2.Models;
using esii_2025_d2.Data; // Namespace for your DbContext
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// using Microsoft.AspNetCore.Authorization; // Uncomment if needed

namespace esii_2025_d2.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize] // Uncomment if needed
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
        // Use English DbSet name
        return await _context.JobProposals.ToListAsync();
    }

    // GET: api/JobProposal/5
    [HttpGet("{id}")]
    public async Task<ActionResult<JobProposal>> GetJobProposal(int id)
    {
        // Use English names for includes and properties
        var jobProposal = await _context.JobProposals
            .Include(p => p.Customer)       // Include Customer
            .Include(p => p.TalentCategory) // Include TalentCategory
            .Include(p => p.Skill)          // Include Skill
            .FirstOrDefaultAsync(p => p.Id == id); // Use Id

        if (jobProposal == null)
        {
            return NotFound();
        }

        return jobProposal;
    }

    // POST: api/JobProposal
    [HttpPost]
    public async Task<ActionResult<JobProposal>> CreateJobProposal(JobProposal newProposal)
    {
        // Validate foreign keys using English names
        if (!string.IsNullOrEmpty(newProposal.CustomerId))
        {
            if (!await _context.Customers.AnyAsync(c => c.Id == newProposal.CustomerId)) // Use Customers DbSet, Id property
                return BadRequest(new { message = $"Customer with ID {newProposal.CustomerId} not found." });
        }

        // Check nullable TalentCategoryId before querying
        if (newProposal.TalentCategoryId.HasValue)
        {
             if (!await _context.TalentCategories.AnyAsync(ct => ct.Id == newProposal.TalentCategoryId)) // Use TalentCategories DbSet, Id property
                return BadRequest(new { message = $"Talent Category with ID {newProposal.TalentCategoryId} not found." });
        }
        else // Handle case where TalentCategoryId is required but provided as null, if applicable
        {
             // return BadRequest(new { message = "Talent Category ID is required." });
        }


        if (!await _context.Skills.AnyAsync(s => s.Id == newProposal.SkillId)) // Use Skills DbSet, Id property
            return BadRequest(new { message = $"Skill with ID {newProposal.SkillId} not found." });

        _context.JobProposals.Add(newProposal);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException /* ex */)
        {
             // Log exception details
            return BadRequest(new { message = "Failed to create job proposal. Check related data." });
        }


        return CreatedAtAction(nameof(GetJobProposal), new { id = newProposal.Id }, newProposal); // Use Id
    }

    // PUT: api/JobProposal/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateJobProposal(int id, JobProposal updatedProposal)
    {
        if (id != updatedProposal.Id) // Use Id
        {
            return BadRequest("Job Proposal ID mismatch.");
        }

        // Re-validate foreign keys
        if (!string.IsNullOrEmpty(updatedProposal.CustomerId))
        {
            if (!await _context.Customers.AnyAsync(c => c.Id == updatedProposal.CustomerId))
                return BadRequest(new { message = $"Customer with ID {updatedProposal.CustomerId} not found." });
        }
         if (updatedProposal.TalentCategoryId.HasValue)
        {
             if (!await _context.TalentCategories.AnyAsync(ct => ct.Id == updatedProposal.TalentCategoryId))
                return BadRequest(new { message = $"Talent Category with ID {updatedProposal.TalentCategoryId} not found." });
        }
        // else { /* Handle null if needed */ }

        if (!await _context.Skills.AnyAsync(s => s.Id == updatedProposal.SkillId))
            return BadRequest(new { message = $"Skill with ID {updatedProposal.SkillId} not found." });


        _context.Entry(updatedProposal).State = EntityState.Modified;

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
        catch (DbUpdateException /* ex */)
        {
             // Log exception details
            return BadRequest(new { message = "Failed to update job proposal. Check related data." });
        }


        return NoContent();
    }

    // DELETE: api/JobProposal/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJobProposal(int id)
    {
        var jobProposal = await _context.JobProposals.FindAsync(id); // Use Id
        if (jobProposal == null)
        {
            return NotFound();
        }

        _context.JobProposals.Remove(jobProposal);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool JobProposalExists(int id)
    {
        return _context.JobProposals.Any(p => p.Id == id); // Use Id
    }
}
