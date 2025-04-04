// esii-2025-d2/Models/TalentSkill.cs
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations; // Keep for potential future attributes

namespace esii_2025_d2.Models;

// Removed [Table("TalentoSkill")] - EF Core will use convention "TalentSkills"

// IMPORTANT: Configure the composite key in ApplicationDbContext.OnModelCreating
// Example: modelBuilder.Entity<TalentSkill>().HasKey(ts => new { ts.TalentId, ts.SkillId });
public class TalentSkill
{
    // Foreign Key Properties (will form the composite key)
    public int SkillId { get; set; } // Was 'codskill'
    public int TalentId { get; set; } // Was 'idtalento'

    // Additional property on the join table
    public int YearsOfExperience { get; set; } // Was 'anosexperiencia'

    // Navigation Properties
    [ForeignKey("SkillId")]
    public virtual Skill Skill { get; set; } = null!; // Was 'codskillNavigation'

    [ForeignKey("TalentId")]
    public virtual Talent Talent { get; set; } = null!; // Was 'idtalentoNavigation'
}
