// esii-2025-d2/Models/Skill.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace esii_2025_d2.Models;

public class Skill
{
    [Key]
    public int Id { get; set; } // Was 'cod'

    [Required]
    [StringLength(100)] // Adjust as needed
    public string Name { get; set; } = null!; // Was 'nome'

    [StringLength(100)] // Adjust as needed
    public string? Area { get; set; } // Was 'area'

    // Navigation Properties
    public virtual ICollection<JobProposal> JobProposals { get; set; } = new List<JobProposal>(); // Was PropostaTrabalhos

    public virtual ICollection<TalentSkill> TalentSkills { get; set; } = new List<TalentSkill>(); // Was TalentoSkills
}
