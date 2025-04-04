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
    public int? TalentCategoryId { get; set; }

    [Required]
    public string UserId { get; set; } = null!; // Foreign key property

    [ForeignKey("TalentCategoryId")]
    public virtual TalentCategory? TalentCategory { get; set; }

    [ForeignKey("UserId")]
    // *** CHANGE: Use ApplicationUser? instead of User? ***
    public virtual ApplicationUser? User { get; set; }

    // Collections of related entities
    public virtual ICollection<Experience> Experiences { get; set; } = new List<Experience>();

    public virtual ICollection<TalentSkill> TalentSkills { get; set; } = new List<TalentSkill>();
}
