using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using esii_2025_d2.Data;
using esii_2025_d2.Models;
using esii_2025_d2.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;

namespace esii_2025_d2.Services
{
    public interface ITalentService
    {
        Task<List<Talent>> GetMyTalentsAsync(string userId);
        Task<List<Talent>> GetCurrentUserTalentsAsync();
        Task<List<Talent>> GetAllTalentsAsync();
        Task<PaginatedResult<Talent>> SearchTalentsAsync(TalentSearchDto searchDto);
        Task<Talent> CreateTalentAsync(Talent talent);
        Task<Talent> UpdateTalentAsync(Talent talent);
        Task DeleteTalentAsync(int id);
        Task<Talent?> GetTalentByIdAsync(int id);
    }

    public class TalentService : ITalentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationStateProvider _authStateProvider;

        public TalentService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, AuthenticationStateProvider authStateProvider)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _authStateProvider = authStateProvider;
        }

        public async Task<List<Talent>> GetMyTalentsAsync(string userId)
        {
            return await _context.Talents
                .Include(t => t.TalentCategory)
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all talents belonging to the currently authenticated user.
        /// This method automatically extracts the user ID from the authentication state and returns only the talents that belong to that user.
        /// </summary>
        /// <returns>A list of talents owned by the current user, including their talent categories, ordered by name.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the user is not authenticated or user ID cannot be determined.</exception>
        public async Task<List<Talent>> GetCurrentUserTalentsAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var userId = authState.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User not authenticated or user ID not found.");
            }

            return await _context.Talents
                .Include(t => t.TalentCategory)
                .Where(t => t.UserId == userId)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<List<Talent>> GetAllTalentsAsync()
        {
            return await _context.Talents
                .Include(t => t.TalentCategory)
                .ToListAsync();
        }

        public async Task<PaginatedResult<Talent>> SearchTalentsAsync(TalentSearchDto searchDto)
        {
            var query = _context.Talents
                .Include(t => t.TalentCategory)
                .Include(t => t.TalentSkills)
                    .ThenInclude(ts => ts.Skill)
                .AsQueryable();

            // Apply search text filter
            if (!string.IsNullOrWhiteSpace(searchDto.SearchText))
            {
                var searchText = searchDto.SearchText.ToLower();
                query = query.Where(t =>
                    t.Name.ToLower().Contains(searchText) ||
                    t.Country.ToLower().Contains(searchText) ||
                    t.Email.ToLower().Contains(searchText));
            }

            // Apply talent category filter
            if (searchDto.TalentCategoryId.HasValue)
            {
                query = query.Where(t => t.TalentCategoryId == searchDto.TalentCategoryId.Value);
            }

            // Apply skill filter
            if (searchDto.SkillId.HasValue)
            {
                query = query.Where(t => t.TalentSkills.Any(ts => ts.SkillId == searchDto.SkillId.Value));
            }

            // Apply country filter
            if (!string.IsNullOrWhiteSpace(searchDto.Country))
            {
                query = query.Where(t => t.Country.ToLower().Contains(searchDto.Country.ToLower()));
            }

            // Apply hourly rate range filter
            if (searchDto.MinHourlyRate.HasValue)
            {
                query = query.Where(t => t.HourlyRate >= searchDto.MinHourlyRate.Value);
            }

            if (searchDto.MaxHourlyRate.HasValue)
            {
                query = query.Where(t => t.HourlyRate <= searchDto.MaxHourlyRate.Value);
            }

            // Apply public status filter
            if (searchDto.IsPublic.HasValue)
            {
                query = query.Where(t => t.IsPublic == searchDto.IsPublic.Value);
            }

            // Get total count before pagination
            var totalItems = await query.CountAsync();

            // Apply sorting
            query = ApplyTalentSorting(query, searchDto.SortBy, searchDto.SortDirection);

            // Apply pagination
            var items = await query
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();

            return new PaginatedResult<Talent>(items, totalItems, searchDto.Page, searchDto.PageSize);
        }

        public async Task<Talent> CreateTalentAsync(Talent talent)
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var userId = authState.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User not authenticated or user ID not found.");
            }

            // Always set the UserId to current user to prevent tampering
            talent.UserId = userId;

            _context.Talents.Add(talent);
            await _context.SaveChangesAsync();
            return talent;
        }

        public async Task<Talent> UpdateTalentAsync(Talent talent)
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var userId = authState.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User not authenticated or user ID not found.");
            }

            // Ensure the talent belongs to the current user
            var existingTalent = await _context.Talents.FirstOrDefaultAsync(t => t.Id == talent.Id && t.UserId == userId);
            if (existingTalent == null)
            {
                throw new UnauthorizedAccessException("Talent not found or you don't have permission to modify it.");
            }

            // Always set the UserId to current user to prevent tampering
            talent.UserId = userId;

            _context.Entry(existingTalent).CurrentValues.SetValues(talent);
            await _context.SaveChangesAsync();
            return talent;
        }

        public async Task DeleteTalentAsync(int id)
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var currentUserId = authState.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
            {
                throw new UnauthorizedAccessException("User not authenticated or user ID not found.");
            }

            var talent = await _context.Talents
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == currentUserId);

            if (talent == null)
            {
                throw new UnauthorizedAccessException("Talent not found or you don't have permission to delete it.");
            }

            _context.Talents.Remove(talent);
            await _context.SaveChangesAsync();
        }

        public async Task<Talent?> GetTalentByIdAsync(int id)
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var currentUserId = authState.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
            {
                throw new UnauthorizedAccessException("User not authenticated or user ID not found.");
            }

            return await _context.Talents
                .Include(t => t.TalentCategory)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == currentUserId);
        }

        private IQueryable<Talent> ApplyTalentSorting(IQueryable<Talent> query, string? sortBy, string sortDirection)
        {
            var isDescending = sortDirection.ToLower() == "desc";

            return sortBy?.ToLower() switch
            {
                "name" => isDescending ? query.OrderByDescending(t => t.Name) : query.OrderBy(t => t.Name),
                "country" => isDescending ? query.OrderByDescending(t => t.Country) : query.OrderBy(t => t.Country),
                "hourlyrate" => isDescending ? query.OrderByDescending(t => t.HourlyRate) : query.OrderBy(t => t.HourlyRate),
                "category" => isDescending ? query.OrderByDescending(t => t.TalentCategory != null ? t.TalentCategory.Name : "") : query.OrderBy(t => t.TalentCategory != null ? t.TalentCategory.Name : ""),
                "email" => isDescending ? query.OrderByDescending(t => t.Email) : query.OrderBy(t => t.Email),
                _ => isDescending ? query.OrderByDescending(t => t.Name) : query.OrderBy(t => t.Name)
            };
        }
    }
}
