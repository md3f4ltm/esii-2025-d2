using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace BlazorApp1.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Existing properties
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        // New properties from Utilizador that might be useful
        public DateTime DataNascimento { get; set; }

        // Navigation properties
        public Cliente? Cliente { get; set; }
        public Talento? Talento { get; set; }
        public List<Skill> Skills { get; set; } = new List<Skill>();
        public List<PropostaTrabalho> PropostasTrabalho { get; set; } = new List<PropostaTrabalho>();
    }
}
