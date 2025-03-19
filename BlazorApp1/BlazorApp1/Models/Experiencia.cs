namespace BlazorApp1.Models
{
    public class Experiencia
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = "";
        public string NomeEmpresa { get; set; } = "";
        public int AnoComeco { get; set; }
        public int? AnoFim { get; set; }
        public int IdTalento { get; set; }

        public Talento? Talento { get; set; }
    }
}
