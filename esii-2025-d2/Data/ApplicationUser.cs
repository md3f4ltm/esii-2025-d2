// esii-2025-d2/Data/User.cs
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System; // For DateOnly
using System.Collections.Generic; // For ICollection
using esii_2025_d2.Models; // Required for Customer, Talent models

namespace esii_2025_d2.Data; // Adjust namespace if needed

// Inherit from IdentityUser
public class ApplicationUser : IdentityUser
{
    // Custom properties from old Utilizador, renamed
    [StringLength(100)]
    public string? Name { get; set; } // Was 'nome'

    // Use DateOnly or DateTime depending on your needs
    [Required]
    [DataType(DataType.Date)]
    [Display(Name="Date of Birth")]
    public DateOnly DateOfBirth { get; set; } // Was 'datanascimento'

    // Email, UserName, PasswordHash etc. are inherited from IdentityUser

    // Navigation properties from old Utilizador, renamed
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>(); // Was 'Clientes'

    public virtual ICollection<Talent> Talents { get; set; } = new List<Talent>(); // Was 'Talentos'
}
