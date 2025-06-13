using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using esii_2025_d2.Data;
using esii_2025_d2.Models;
using esii_2025_d2.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using System.Linq.Expressions;

namespace esii_2025_d2.Services
{
    public interface ITalentService
    {
        Task<List<Talent>> GetMyTalentsAsync(string userId);
        Task<List<Talent>> GetAllTalentsAsync();
        Task<PaginatedResult<Talent>> SearchTalentsAsync(TalentSearchDto searchDto);
    }

    public class TalentService : ITalentService
    {
        private readonly ApplicationDbContext _context;

        public TalentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Talent>> GetMyTalentsAsync(string userId)
        {
            return await _context.Talents
                .Where(t => t.UserId == userId)
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
