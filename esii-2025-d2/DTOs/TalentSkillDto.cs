using System.ComponentModel.DataAnnotations;
using esii_2025_d2.Models;

namespace esii_2025_d2.DTOs
{
    public class TalentSkillDto
    {
        public int TalentId { get; set; }
        
        [Required(ErrorMessage = "Skill is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a skill")]
        public int SkillId { get; set; }
        
        [Required(ErrorMessage = "Years of experience is required")]
        [Range(1, 50, ErrorMessage = "Years of experience must be between 1 and 50")]
        public int YearsOfExperience { get; set; }
        
        public Skill? Skill { get; set; }
    }
} 