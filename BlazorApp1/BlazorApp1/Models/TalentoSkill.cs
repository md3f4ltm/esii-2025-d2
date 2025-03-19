using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models
{
    public class TalentoSkill
    {
        [Key]
        public int Id { get; set; }
        public int CodSkill { get; set; }
        public int IdTalento { get; set; }
        public int AnosExperiencia { get; set; }

        public Skill? Skill { get; set; }
        public Talento? Talento { get; set; }
    }
}
