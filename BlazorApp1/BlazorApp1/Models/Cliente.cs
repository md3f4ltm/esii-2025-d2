using System;

namespace BlazorApp1.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Empresa { get; set; } = "";
        public string NumeroTelefone { get; set; } = "";
        // Foreign key property
        public string UserId { get; set; } = "";

        // Navigation property for related ApplicationUser
        public ApplicationUser? User { get; set; }
    }
}
