using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity; // Required namespace

namespace ESII2025d2.Models;

// Inherit from IdentityUser. No need for [Table] attribute here,
// IdentityDbContext handles table mapping (usually to 'AspNetUsers').
// If you MUST name the table "Utilizador", configure it in OnModelCreating.
public class Utilizador : IdentityUser // Inherit from IdentityUser
{
    // [Key] is removed. Id (string) from IdentityUser is the PK.

    // Keep your custom properties
    [Required] // Add validation as needed
    [StringLength(100)]
    public string nome { get; set; } = null!; // Can store full name or first name

    public DateOnly datanascimento { get; set; }

    // Email, UserName, PasswordHash etc. are inherited from IdentityUser

    // Keep your relationships
    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
    public virtual ICollection<Talento> Talentos { get; set; } = new List<Talento>();
}
