using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models
{
    public class CategoriaTalento
    {
        [Key]
        public int Id { get; set; } // Primary key
        public int Cod { get; set; }
        public string Nome { get; set; } = "";
        public List<Talento> Talentos { get; set; } = new List<Talento>();
    }
}
