using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace esii_2025_d2.Models;

public class Experience
{
    [Key]
    public int Id { get; set; } // Was 'id'

    [Required]
    [StringLength(200)] 
    public string Title { get; set; } = null!; // Was 'titulo'

    [Required]
    [StringLength(150)] 
    public string CompanyName { get; set; } = null!; 

    public int StartYear { get; set; }

    public int? EndYear { get; set; } 

    public int TalentId { get; set; }

    [ForeignKey("TalentId")] 
    public virtual Talent Talent { get; set; } = null!; 
}
