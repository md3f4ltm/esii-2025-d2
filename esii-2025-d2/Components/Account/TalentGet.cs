using System.ComponentModel;
using esii_2025_d2.Data;
using esii_2025_d2.Models;

public class TalentGet{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TalentGet(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<Talent>> GetTalentOfUser(){

        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if(userId == null){
            return new List<Talent>;
        }

    }

}