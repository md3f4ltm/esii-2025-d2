// esii-2025-d2/Models/Experience.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace esii_2025_d2.Models;

// Removed [Table("Experiencia")] - EF Core will use convention "Experiences"
public class Experience
{
    [Key]
    public int Id { get; set; } // Was 'id'

    [Required]
    [StringLength(200)] // Adjust length as needed
    public string Title { get; set; } = null!; // Was 'titulo'

    [Required]
    [StringLength(150)] // Adjust length as needed
    public string CompanyName { get; set; } = null!; // Was 'nomeempresa'

    public int StartYear { get; set; } // Was 'anocomeco'

    public int? EndYear { get; set; } // Was 'anofim' (nullable kept)

    // --- Foreign Key and Navigation Property for Talent ---
    public int TalentId { get; set; } // Was 'idtalento'

    [ForeignKey("TalentId")] // Point to the foreign key property above
    public virtual Talent Talent { get; set; } = null!; // Was 'idtalentoNavigation', assumes Talento->Talent
}
