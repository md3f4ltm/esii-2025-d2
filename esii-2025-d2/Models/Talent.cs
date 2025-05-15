// esii-2025-d2/Models/Talent.cs
using esii_2025_d2.Data; // Needed for ApplicationUser class
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace esii_2025_d2.Models;

public class Talent
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Country { get; set; } = null!;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = null!;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal HourlyRate { get; set; }

    // --- Foreign Keys and Navigation Properties ---

    // If a TalentCategory is always required, make TalentCategoryId non-nullable (int)
    // and apply the [Required] attribute here.
    [Required(ErrorMessage = "O ID da Categoria de Talento é obrigatório.")] // Validation for the FK ID
    [Display(Name = "Talent Category ID")]
    public int TalentCategoryId { get; set; } // Changed from int? to int

    [Required]
    public string UserId { get; set; } = null!; // Foreign key property

    // Remove [Required] from the navigation property.
    // The relationship is managed by TalentCategoryId.
    [ForeignKey("TalentCategoryId")]
    [Display(Name = "Categoria de Talento")]
    public virtual TalentCategory? TalentCategory { get; set; } // Remains nullable, EF Core handles loading it

    [ForeignKey("UserId")]
    public virtual ApplicationUser? User { get; set; }

    // Collections of related entities
    public virtual ICollection<Experience> Experiences { get; set; } = new List<Experience>();
    public virtual ICollection<TalentSkill> TalentSkills { get; set; } = new List<TalentSkill>();
}
