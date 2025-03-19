using System;

namespace BlazorApp1.Models
{
    public class PropostaTrabalho
    {
        public int Cod { get; set; }
        public int CodSkill { get; set; }
        public int CodTalento { get; set; }
        public string Nome { get; set; } = "";
        public int NumTotalHoras { get; set; }
        public string? Descricao { get; set; }

        public Skill? Skill { get; set; }
        public Talento? Talento { get; set; }
        // Navigation property for related ApplicationUser
        public ApplicationUser? User { get; set; }
    }
}
