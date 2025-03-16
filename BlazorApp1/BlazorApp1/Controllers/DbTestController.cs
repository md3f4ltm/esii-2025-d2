using BlazorApp1.Data;
using Microsoft.AspNetCore.Mvc;
using BlazorApp1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DbTestController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DbTestController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> TestDatabase()
    {
        try
        {
            bool canConnect = _context.Database.CanConnect();
            int userCount = await _context.Users.CountAsync();

            return Ok(new
            {
                ConnectionSuccessful = canConnect,
                UserCount = userCount,
                DatabaseProvider = _context.Database.ProviderName
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    // Add a new endpoint to create a test user

    [HttpPost("create-test-user")]
    public async Task<IActionResult> CreateTestUser()
    {
        try
        {
            var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();

            // Check if user exists
            var testUser = await userManager.FindByEmailAsync("test2@example.com");

            if (testUser == null)
            {
                // Create test user
                testUser = new Models.ApplicationUser
                {
                    UserName = "testuser2",
                    Email = "test2@example.com",
                    EmailConfirmed = true,
                    FirstName = "Test2",
                    LastName = "User2"
                };

                var result = await userManager.CreateAsync(testUser, "Test123!");

                if (result.Succeeded)
                {
                    return Ok(new { Success = true, Message = "Test user created successfully" });
                }
                else
                {
                    return BadRequest(new { Success = false, Errors = result.Errors.Select(e => e.Description) });
                }
            }
            else
            {
                return Ok(new { Success = true, Message = "User already exists" });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Error = ex.Message });
        }
    }

    // Add a new endpoint to check tables

    [HttpGet("tables")]
    public IActionResult GetTables()
    {
        try
        {
            var tables = _context.Model.GetEntityTypes()
                .Select(t => t.GetTableName())
                .ToList();

            return Ok(new
            {
                Tables = tables,
                ConnectionString = _context.Database.GetConnectionString()
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}