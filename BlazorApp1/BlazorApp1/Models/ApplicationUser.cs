using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace BlazorApp1.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        // New properties from Utilizador that might be useful
        // Adicione este código se precisar do campo na classe, mas marcando-o como opcional
        public DateTime? DataNascimento { get; set; }

        // Navigation properties
        public Cliente? Cliente { get; set; }
        public Talento? Talento { get; set; }
        public List<Skill> Skills { get; set; } = new List<Skill>();
        public List<PropostaTrabalho> PropostasTrabalho { get; set; } = new List<PropostaTrabalho>();
    }
}
