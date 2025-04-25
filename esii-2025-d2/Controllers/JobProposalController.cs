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
// [Authorize] // Consider adding authorization if needed
public class JobProposalController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public JobProposalController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/JobProposal
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobProposal>>> GetJobProposals() // Existing method
    {
        // Use English DbSet name
        return await _context.JobProposals.ToListAsync();
    }

    // *** START: NEW ENDPOINT FOR FEED ***
    // GET: api/JobProposal/GetAllProposals
    [HttpGet("GetAllProposals")]
    public async Task<ActionResult<IEnumerable<JobProposal>>> GetAllProposals()
    {
        // Consider adding filtering (e.g., only show 'Open' proposals) or pagination
        // Also consider which related data to include (e.g., Customer info)
        // return await _context.JobProposals.Include(jp => jp.Customer).Where(jp => jp.Status == "Open").ToListAsync();
        return await _context.JobProposals.ToListAsync(); // Simple version for now
    }
    // *** END: NEW ENDPOINT FOR FEED ***

    // GET: api/JobProposal/5
    [HttpGet("{id}")]
    public async Task<ActionResult<JobProposal>> GetJobProposal(int id)
    {
        // Use English DbSet name
        var jobProposal = await _context.JobProposals.FindAsync(id);

        if (jobProposal == null)
        {
            return NotFound();
        }

        return jobProposal;
    }

    // PUT: api/JobProposal/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutJobProposal(int id, JobProposal jobProposal)
    {
        if (id != jobProposal.Id)
        {
            return BadRequest();
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
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<JobProposal>> PostJobProposal(JobProposal jobProposal)
    {
        _context.JobProposals.Add(jobProposal);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetJobProposal", new { id = jobProposal.Id }, jobProposal);
    }

    // DELETE: api/JobProposal/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJobProposal(int id)
    {
        var jobProposal = await _context.JobProposals.FindAsync(id);
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
        return _context.JobProposals.Any(e => e.Id == id);
    }
}
