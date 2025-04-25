// esii-2025-d2/Models/JobProposal.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace esii_2025_d2.Models;

public class JobProposal
{
    [Key]
    public int Id { get; set; } // Was 'cod'

    [Required]
    [StringLength(150)] // Adjust as needed
    public string Name { get; set; } = null!; // Was 'nome'

    public int TotalHours { get; set; } // Was 'numtotalhoras'

    [StringLength(500)] // Adjust as needed
    public string? Description { get; set; } // Was 'descricao'

    // --- Foreign Keys and Navigation Properties ---
    public int SkillId { get; set; } // Was 'codskill'

    public string? CustomerId { get; set; } // Was 'cliente_id' (nullable kept, type string matches Customer.Id)

    public int? TalentCategoryId { get; set; } // Was 'cattalento_cod' (nullable kept)


    [ForeignKey("TalentCategoryId")]
    public virtual TalentCategory? TalentCategory { get; set; } // Was 'cattalento_codNavigation'

    [ForeignKey("CustomerId")]
    public virtual Customer? Customer { get; set; } // Was 'cliente'

    [ForeignKey("SkillId")]
    public virtual Skill Skill { get; set; } = null!; // Was 'codskillNavigation'
}
