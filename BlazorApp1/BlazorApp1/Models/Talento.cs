using System.Collections.Generic;

namespace BlazorApp1.Models
{
    public class Talento
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
        public string Pais { get; set; } = "";
        public string Email { get; set; } = "";
        public decimal PrecoHora { get; set; }
        public int? CodCategoriaTalento { get; set; }
        public string UserId { get; set; } = ""; // Changed to string to match ApplicationUser Id

        public CategoriaTalento? CategoriaTalento { get; set; }
        public ApplicationUser? User { get; set; }
        public List<Experiencia> Experiencias { get; set; } = new List<Experiencia>();
        public List<TalentoSkill> TalentoSkills { get; set; } = new List<TalentoSkill>();
    }
}
