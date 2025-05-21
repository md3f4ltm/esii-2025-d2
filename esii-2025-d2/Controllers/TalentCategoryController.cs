// esii-2025-d2/Controllers/TalentCategoryController.cs
using esii_2025_d2.Models;
using esii_2025_d2.Data; // Namespace for your DbContext
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
// using Microsoft.AspNetCore.Authorization; // Uncomment if needed

namespace esii_2025_d2.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize] // Uncomment if needed
public class TalentCategoryController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TalentCategoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/TalentCategory
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TalentCategory>>> GetTalentCategories()
    {
        return await _context.TalentCategories.ToListAsync(); // Use English DbSet name
    }

    // GET: api/TalentCategory/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TalentCategory>> GetTalentCategory(int id)
    {
        var talentCategory = await _context.TalentCategories.FindAsync(id); // Use English DbSet name & Id

        if (talentCategory == null)
        {
            return NotFound();
        }

        return talentCategory;
    }

    // POST: api/TalentCategory
    [HttpPost]
    public async Task<ActionResult<TalentCategory>> CreateTalentCategory(TalentCategory newCategory)
    {
        if (await _context.TalentCategories.AnyAsync(c => c.Name == newCategory.Name)) // Use Name
        {
            return Conflict(new { message = "A talent category with this name already exists." });
        }

        _context.TalentCategories.Add(newCategory);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTalentCategory), new { id = newCategory.Id }, newCategory); // Use Id
    }

    // PUT: api/TalentCategory/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTalentCategory(int id, TalentCategory updatedCategory)
    {
        if (id != updatedCategory.Id) // Use Id
        {
            return BadRequest("Talent category ID mismatch.");
        }

        _context.Entry(updatedCategory).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TalentCategoryExists(id))
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

    // DELETE: api/TalentCategory/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTalentCategory(int id)
    {
        var talentCategory = await _context.TalentCategories.FindAsync(id);
        if (talentCategory == null)
        {
            return NotFound();
        }

        _context.TalentCategories.Remove(talentCategory);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TalentCategoryExists(int id)
    {
        return _context.TalentCategories.Any(c => c.Id == id); // Use Id
    }
}
