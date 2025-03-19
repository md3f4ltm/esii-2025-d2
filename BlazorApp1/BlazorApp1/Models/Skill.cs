using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models
{
    public class Skill
    {
        [Key]
        public int Cod { get; set; }
        public string Nome { get; set; } = "";
        public string? Area { get; set; }
        public string? UserId { get; set; } // Changed to string to match ApplicationUser Id

        public ApplicationUser? User { get; set; }
        public List<TalentoSkill> TalentoSkills { get; set; } = new List<TalentoSkill>();
    }
}
