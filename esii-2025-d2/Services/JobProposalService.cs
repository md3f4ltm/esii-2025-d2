using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using esii_2025_d2.Data;
using esii_2025_d2.Models;
using esii_2025_d2.DTOs;
using System.Linq.Expressions;

namespace esii_2025_d2.Services
{
    public interface IJobProposalService
    {
        Task<List<JobProposal>> GetAllJobProposalsAsync();
        Task<List<JobProposal>> GetJobProposalsByCustomerIdAsync(string customerId);
        Task<JobProposal?> GetJobProposalByIdAsync(int id);
        Task<JobProposal?> CreateJobProposalAsync(JobProposal jobProposal);
        Task<bool> UpdateJobProposalAsync(JobProposal jobProposal);
        Task<bool> DeleteJobProposalAsync(int id, string customerId);
        Task<PaginatedResult<JobProposal>> SearchJobProposalsAsync(JobProposalSearchDto searchDto);
    }

    public class JobProposalService : IJobProposalService
    {
        private readonly ApplicationDbContext _context;

        public JobProposalService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<JobProposal>> GetAllJobProposalsAsync()
        {
            return await _context.JobProposals
                .Include(jp => jp.Skill)
                .Include(jp => jp.TalentCategory)
                .Include(jp => jp.Customer)
                .ToListAsync();
        }

        public async Task<List<JobProposal>> GetJobProposalsByCustomerIdAsync(string customerId)
        {
            return await _context.JobProposals
                .Include(jp => jp.Skill)
                .Include(jp => jp.TalentCategory)
                .Where(jp => jp.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<JobProposal?> GetJobProposalByIdAsync(int id)
        {
            return await _context.JobProposals
                .Include(jp => jp.Skill)
                .Include(jp => jp.TalentCategory)
                .Include(jp => jp.Customer)
                .FirstOrDefaultAsync(jp => jp.Id == id);
        }

        public async Task<JobProposal?> CreateJobProposalAsync(JobProposal jobProposal)
        {
            _context.JobProposals.Add(jobProposal);
            await _context.SaveChangesAsync();
            return jobProposal;
        }

        public async Task<bool> UpdateJobProposalAsync(JobProposal jobProposal)
        {
            var existing = await _context.JobProposals
                .FirstOrDefaultAsync(jp => jp.Id == jobProposal.Id);

            if (existing == null)
                return false;

            // Ensure we're not changing the customer
            jobProposal.CustomerId = existing.CustomerId;

            _context.Entry(existing).CurrentValues.SetValues(jobProposal);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteJobProposalAsync(int id, string customerId)
        {
            var jobProposal = await _context.JobProposals
                .FirstOrDefaultAsync(jp => jp.Id == id && jp.CustomerId == customerId);

            if (jobProposal == null)
                return false;

            _context.JobProposals.Remove(jobProposal);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedResult<JobProposal>> SearchJobProposalsAsync(JobProposalSearchDto searchDto)
        {
            var query = _context.JobProposals
                .Include(jp => jp.Skill)
                .Include(jp => jp.TalentCategory)
                .Include(jp => jp.Customer)
                .AsQueryable();

            // Apply search text filter
            if (!string.IsNullOrWhiteSpace(searchDto.SearchText))
            {
                var searchText = searchDto.SearchText.ToLower();
                query = query.Where(jp =>
                    jp.Name.ToLower().Contains(searchText) ||
                    (jp.Description != null && jp.Description.ToLower().Contains(searchText)));
            }

            // Apply skill filter
            if (searchDto.SkillId.HasValue)
            {
                query = query.Where(jp => jp.SkillId == searchDto.SkillId.Value);
            }

            // Apply talent category filter
            if (searchDto.TalentCategoryId.HasValue)
            {
                query = query.Where(jp => jp.TalentCategoryId == searchDto.TalentCategoryId.Value);
            }

            // Apply customer filter
            if (!string.IsNullOrWhiteSpace(searchDto.CustomerId))
            {
                query = query.Where(jp => jp.CustomerId == searchDto.CustomerId);
            }

            // Apply total hours range filter
            if (searchDto.MinTotalHours.HasValue)
            {
                query = query.Where(jp => jp.TotalHours >= searchDto.MinTotalHours.Value);
            }

            if (searchDto.MaxTotalHours.HasValue)
            {
                query = query.Where(jp => jp.TotalHours <= searchDto.MaxTotalHours.Value);
            }

            // Get total count before pagination
            var totalItems = await query.CountAsync();

            // Apply sorting
            query = ApplyJobProposalSorting(query, searchDto.SortBy, searchDto.SortDirection);

            // Apply pagination
            var items = await query
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();

            return new PaginatedResult<JobProposal>(items, totalItems, searchDto.Page, searchDto.PageSize);
        }

        private IQueryable<JobProposal> ApplyJobProposalSorting(IQueryable<JobProposal> query, string? sortBy, string sortDirection)
        {
            var isDescending = sortDirection.ToLower() == "desc";

            return sortBy?.ToLower() switch
            {
                "name" => isDescending ? query.OrderByDescending(jp => jp.Name) : query.OrderBy(jp => jp.Name),
                "totalhours" => isDescending ? query.OrderByDescending(jp => jp.TotalHours) : query.OrderBy(jp => jp.TotalHours),
                "skill" => isDescending ? query.OrderByDescending(jp => jp.Skill.Name) : query.OrderBy(jp => jp.Skill.Name),
                "category" => isDescending ? query.OrderByDescending(jp => jp.TalentCategory != null ? jp.TalentCategory.Name : "") : query.OrderBy(jp => jp.TalentCategory != null ? jp.TalentCategory.Name : ""),
                "customer" => isDescending ? query.OrderByDescending(jp => jp.Customer != null ? jp.Customer.Company : "") : query.OrderBy(jp => jp.Customer != null ? jp.Customer.Company : ""),
                _ => isDescending ? query.OrderByDescending(jp => jp.Name) : query.OrderBy(jp => jp.Name)
            };
        }
    }
}
