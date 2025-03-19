using System.Collections.Generic;

namespace BlazorApp1.Models
{
    public class TalentoSkill
    {
        public int CodSkill { get; set; }
        public int IdTalento { get; set; }
        public int AnosExperiencia { get; set; }

        public Skill? Skill { get; set; }
        public Talento? Talento { get; set; }
    }
}
