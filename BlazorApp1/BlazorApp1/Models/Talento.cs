using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Add this line

namespace BlazorApp1.Models
{
    public class Talento
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string Nome { get; set; } = "";
        public decimal PrecoHora { get; set; }
        public int? CodCategoriaTalento { get; set; }
        public string UserId { get; set; } = "";
        public string Pais { get; set; } = ""; // If you added this earlier

        public CategoriaTalento? CategoriaTalento { get; set; }
        public ApplicationUser? User { get; set; }
        public List<Experiencia> Experiencias { get; set; } = new List<Experiencia>();
        public List<TalentoSkill> TalentoSkills { get; set; } = new List<TalentoSkill>();
    }
}
