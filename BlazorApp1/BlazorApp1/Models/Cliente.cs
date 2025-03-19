using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }
        public string Empresa { get; set; } = "";
        public string NumeroTelefone { get; set; } = "";
        // Foreign key property
        public string UserId { get; set; } = "";

        // Navigation property for related ApplicationUser
        public ApplicationUser? User { get; set; }
    }
}
