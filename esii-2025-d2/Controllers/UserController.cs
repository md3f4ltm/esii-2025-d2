// esii-2025-d2/Controllers/UserController.cs
using esii_2025_d2.Data; // For ApplicationDbContext and ApplicationUser
using esii_2025_d2.Models; // For Talent
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace esii_2025_d2.Controllers;

// DTO is fine as is
public class UserInfoDto
{
    public string Id { get; set; } = null!;
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string Name { get; set; } = null!;
}

[Route("api/[controller]")] // Route will likely be /api/User based on class name
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    // *** CHANGE: Use UserManager<ApplicationUser> ***
    private readonly UserManager<ApplicationUser> _userManager;

    // *** CHANGE: Inject UserManager<ApplicationUser> ***
    public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: api/User
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<UserInfoDto>>> GetUsers()
    {
        // Use _context.Users (comes from IdentityDbContext<ApplicationUser>)
        // Select Name property (which is now in ApplicationUser)
        return await _context.Users
            .Select(u => new UserInfoDto {
                Id = u.Id,
                Email = u.Email,
                Username = u.UserName,
                Name = u.Name // Select Name from ApplicationUser
            })
            .ToListAsync();
    }

    // GET: api/User/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<UserInfoDto>> GetUser(string id) // Method name kept as GetUser
    {
        // *** CHANGE: user variable type is ApplicationUser ***
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        var userInfo = new UserInfoDto
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.UserName,
            Name = user.Name // Use Name from ApplicationUser
        };
        return Ok(userInfo);
    }

    // PUT: api/User/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, UserInfoDto updatedUserDto) // Method name kept as UpdateUser
    {
        if (id != updatedUserDto.Id)
        {
            return BadRequest("ID mismatch.");
        }

        var currentUserId = _userManager.GetUserId(User); // This gets the ClaimsPrincipal User
        // Authorization check - This part using ClaimsPrincipal User is okay
        if (currentUserId != id && !User.IsInRole("Admin"))
        {
           return Forbid();
        }

        // *** CHANGE: user variable type is ApplicationUser ***
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // Update custom properties on ApplicationUser
        user.Name = updatedUserDto.Name;
        // user.DateOfBirth = updatedUserDto.DateOfBirth; // If you add DateOfBirth to DTO

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }

        // Update related Talent data
        var talent = await _context.Talents.FirstOrDefaultAsync(t => t.UserId == id);
        if (talent != null)
        {
            bool talentChanged = false;
            if (talent.Name != user.Name)
            {
                 talent.Name = user.Name;
                 talentChanged = true;
            }
            if (talent.Email != user.Email)
            {
                 talent.Email = user.Email ?? string.Empty;
                 talentChanged = true;
            }

            if(talentChanged)
            {
                 _context.Entry(talent).State = EntityState.Modified;
            }
        }
        await _context.SaveChangesAsync(); // Save changes once

        return NoContent();
    }

    // DELETE: api/User/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id) // Method name kept as DeleteUser
    {
       var currentUserId = _userManager.GetUserId(User);
        if (currentUserId != id && !User.IsInRole("Admin"))
        {
           return Forbid();
        }

        // *** CHANGE: user variable type is ApplicationUser ***
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // Handling related data is the same logic
        var relatedTalents = await _context.Talents.Where(t => t.UserId == id).ToListAsync();
        if (relatedTalents.Any())
        {
              return BadRequest(new { message = "Cannot delete user with associated Talents." });
        }
         var relatedCustomers = await _context.Customers.Where(c => c.UserId == id).ToListAsync();
        if (relatedCustomers.Any())
        {
             return BadRequest(new { message = "Cannot delete user with associated Customers." });
        }

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
             var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { message = "Failed to delete user.", errors });
        }

        return NoContent();
    }
    
    [HttpPost("{userId}/skills")]
    public async Task<IActionResult> AddSkillToUser(string userId, [FromBody] Skill skill)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        if (user.Skills == null)
        {
            user.Skills = new List<Skill>();
        }

        user.Skills.Add(skill);
        await _context.SaveChangesAsync();

        return Ok();
    }

}
