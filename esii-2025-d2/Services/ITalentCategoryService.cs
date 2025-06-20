using esii_2025_d2.Models;

namespace esii_2025_d2.Services
{
    public interface ITalentCategoryService
    {
        Task<List<TalentCategory>> GetAllTalentCategoriesAsync();
        Task<TalentCategory?> GetTalentCategoryByIdAsync(int id);
        Task<TalentCategory> CreateTalentCategoryAsync(TalentCategory talentCategory);
        Task<bool> UpdateTalentCategoryAsync(TalentCategory talentCategory);
        Task<bool> DeleteTalentCategoryAsync(int id);
    }
}
