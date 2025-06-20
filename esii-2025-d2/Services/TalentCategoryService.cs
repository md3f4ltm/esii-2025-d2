using Microsoft.EntityFrameworkCore;
using esii_2025_d2.Data;
using esii_2025_d2.Models;

namespace esii_2025_d2.Services
{
    public class TalentCategoryService : ITalentCategoryService
    {
        private readonly ApplicationDbContext _context;

        public TalentCategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TalentCategory>> GetAllTalentCategoriesAsync()
        {
            return await _context.TalentCategories
                .OrderBy(tc => tc.Name)
                .ToListAsync();
        }

        public async Task<TalentCategory?> GetTalentCategoryByIdAsync(int id)
        {
            return await _context.TalentCategories
                .FirstOrDefaultAsync(tc => tc.Id == id);
        }

        public async Task<TalentCategory> CreateTalentCategoryAsync(TalentCategory talentCategory)
        {
            _context.TalentCategories.Add(talentCategory);
            await _context.SaveChangesAsync();
            return talentCategory;
        }

        public async Task<bool> UpdateTalentCategoryAsync(TalentCategory talentCategory)
        {
            var existingCategory = await _context.TalentCategories
                .FirstOrDefaultAsync(tc => tc.Id == talentCategory.Id);

            if (existingCategory == null)
                return false;

            _context.Entry(existingCategory).CurrentValues.SetValues(talentCategory);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTalentCategoryAsync(int id)
        {
            var talentCategory = await _context.TalentCategories
                .FirstOrDefaultAsync(tc => tc.Id == id);

            if (talentCategory == null)
                return false;

            _context.TalentCategories.Remove(talentCategory);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
