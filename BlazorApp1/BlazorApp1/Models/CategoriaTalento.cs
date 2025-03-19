using System.Collections.Generic;

namespace BlazorApp1.Models
{
    public class CategoriaTalento
    {
        public int Cod { get; set; }
        public string Nome { get; set; } = "";
        public List<Talento> Talentos { get; set; } = new List<Talento>();
    }
}
