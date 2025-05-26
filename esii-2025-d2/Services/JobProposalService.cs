using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using esii_2025_d2.Data;
using esii_2025_d2.Models;

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
    }
}
