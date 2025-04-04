// esii-2025-d2/Models/TalentCategory.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace esii_2025_d2.Models;

// Removed [Table("CategoriaTalento")] - EF Core will use convention "TalentCategories"
public class TalentCategory
{
    [Key]
    public int Id { get; set; } // Was 'cod'

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!; // Was 'nome'

    // Navigation property for JobProposals (assuming PropostaTrabalho becomes JobProposal)
    public virtual ICollection<JobProposal> JobProposals { get; set; } = new List<JobProposal>(); // Was PropostaTrabalhos

    // Navigation property for Talents (assuming Talento becomes Talent)
    public virtual ICollection<Talent> Talents { get; set; } = new List<Talent>(); // Was Talentos
}
