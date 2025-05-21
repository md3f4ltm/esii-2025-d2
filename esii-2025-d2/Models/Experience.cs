using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;

namespace esii_2025_d2.Models;

public class Experience
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(200)]  
    public string Title { get; set; } = null!; 

    [Required(ErrorMessage = "O nome da empresa é obrigatório.")]
    [StringLength(150)]  
    public string CompanyName { get; set; } = null!; 

    [Required(ErrorMessage = "O ano de início é obrigatório.")]
    [Range(1900, 2100, ErrorMessage = "Ano de início deve estar entre 1900 e 2100")]
    public int StartYear { get; set; }

    // EndYear can be null if it's an ongoing experience
    [Range(1900, 2100, ErrorMessage = "Ano de término deve estar entre 1900 e 2100")]
    public int? EndYear { get; set; }  

    [Required(ErrorMessage = "A referência ao talento (TalentId) é obrigatória.")]
    public int TalentId { get; set; }

    [ForeignKey("TalentId")]  
    public virtual Talent? Talent { get; set; }

    // Validate that EndYear is greater than or equal to StartYear
    public bool IsEndYearValid()
    {
        return !EndYear.HasValue || EndYear.Value >= StartYear;
    }

    // Check for overlapping experiences with other experiences for the same talent
    public bool IsOverlapping(IEnumerable<Experience> otherExperiences)
    {
        // Skip the current experience when checking for overlaps
        var experiences = otherExperiences.Where(e => e.Id != this.Id && e.TalentId == this.TalentId);
        
        foreach (var exp in experiences)
        {
            // If this experience has no end year (current job), it overlaps with any experience that starts after StartYear
            if (!this.EndYear.HasValue)
            {
                if (exp.StartYear >= this.StartYear)
                {
                    return true;
                }
            }
            // If the other experience has no end year (current job)
            else if (!exp.EndYear.HasValue)
            {
                if (this.StartYear >= exp.StartYear || 
                    (this.EndYear.HasValue && this.EndYear.Value >= exp.StartYear))
                {
                    return true;
                }
            }
            // Both experiences have end years
            else
            {
                // Check for any overlap in date ranges
                if ((this.StartYear >= exp.StartYear && this.StartYear <= exp.EndYear.Value) ||
                    (this.EndYear.Value >= exp.StartYear && this.EndYear.Value <= exp.EndYear.Value) ||
                    (this.StartYear <= exp.StartYear && this.EndYear.Value >= exp.EndYear.Value))
                {
                    return true;
                }
            }
        }
        
        return false;
    }
}
