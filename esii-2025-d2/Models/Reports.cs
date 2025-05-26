namespace esii_2025_d2.Models
{
    public class CategoryCountryReport
    {
        public string CategoryName { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public decimal AverageMonthlyRate { get; set; }
        public int TalentCount { get; set; }
    }

    public class SkillReport
    {
        public string SkillName { get; set; } = string.Empty;
        public string? Area { get; set; }
        public decimal AverageMonthlyRate { get; set; }
        public int TalentCount { get; set; }
    }
}
