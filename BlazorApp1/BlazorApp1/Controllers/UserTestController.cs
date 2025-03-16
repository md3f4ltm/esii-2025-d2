using BlazorApp1.Data;
using BlazorApp1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserTestController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UserTestController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetUsers()
    {
        return await _context.Users
            .Select(u => new
            {
                u.Id,
                u.UserName,
                u.Email,
                u.FirstName,
                u.LastName
            })
            .ToListAsync();
    }
}