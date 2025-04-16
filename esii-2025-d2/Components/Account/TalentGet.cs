using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using esii_2025_d2.Data;
using esii_2025_d2.Models;

public class TalentGet
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TalentGet(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<Talent>> GetTalentOfUser()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return new List<Talent>();
        }

        return await _context.Talents
            .Where(t => t.UserId == userId)
            .Include(t => t.TalentCategory) // se quiseres usar categoria depois
            .ToListAsync();
    }
}