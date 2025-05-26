// esii-2025-d2/Controllers/CustomerController.cs
using esii_2025_d2.Models;
using esii_2025_d2.Data; // Namespace for your DbContext and User
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace esii_2025_d2.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CustomerController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Customer
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
    {
        return await _context.Customers.ToListAsync(); // Use English DbSet name
    }

    // GET: api/Customer/5 (parameter type is string matching the key type)
    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetCustomer(string id)
    {
        var customer = await _context.Customers
            .Include(c => c.JobProposals) // Use English navigation property
            .Include(c => c.User)         // Use English navigation property
            .SingleOrDefaultAsync(c => c.Id == id); // Use Id property

        if (customer == null)
        {
            return NotFound();
        }

        return customer;
    }

    // GET: api/Customer/user/{userId}
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<Customer>> GetCustomerByUserId(string userId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Unauthorized(new { message = "User not authenticated." });
        }

        // Only allow users to access their own customer data (or admins can access any)
        if (userId != currentUserId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var customer = await _context.Customers
            .Include(c => c.JobProposals)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (customer == null)
        {
            return NotFound(new { message = $"Customer with User ID {userId} not found." });
        }

        return customer;
    }

    // POST: api/Customer
    [HttpPost]
    public async Task<ActionResult<Customer>> CreateCustomer(Customer newCustomer)
    {
        // Optional: Validate that the UserId exists
        if (await _context.Users.FindAsync(newCustomer.UserId) == null)
        {
             return BadRequest(new { message = $"User with ID {newCustomer.UserId} not found." });
        }

        _context.Customers.Add(newCustomer);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException /* ex */)
        {
            // Log exception details
            return BadRequest(new { message = "Failed to create customer. Check related data." });
        }

        return CreatedAtAction(nameof(GetCustomer), new { id = newCustomer.Id }, newCustomer); // Use Id
    }

    // PUT: api/Customer/5 (parameter type is string matching the key type)
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(string id, Customer updatedCustomer)
    {
        if (id != updatedCustomer.Id) // Use Id
        {
            return BadRequest("Customer ID mismatch.");
        }

        // Optional: Re-validate UserId if it's allowed to change
        if (await _context.Users.FindAsync(updatedCustomer.UserId) == null)
        {
             return BadRequest(new { message = $"User with ID {updatedCustomer.UserId} not found." });
        }

        _context.Entry(updatedCustomer).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CustomerExists(id))
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
            return BadRequest(new { message = "Failed to update customer. Check related data." });
        }

        return NoContent();
    }

    // DELETE: api/Customer/5 (parameter type is string matching the key type)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(string id)
    {
        var customer = await _context.Customers
            .Include(c => c.JobProposals) // Use English navigation property
            .FirstOrDefaultAsync(c => c.Id == id); // Use Id

        if (customer == null)
        {
            return NotFound();
        }

        // Remove dependent records first
        if (customer.JobProposals != null && customer.JobProposals.Any())
        {
            _context.JobProposals.RemoveRange(customer.JobProposals); // Use English DbSet name
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CustomerExists(string id)
    {
        return _context.Customers.Any(c => c.Id == id); // Use Id
    }
}
