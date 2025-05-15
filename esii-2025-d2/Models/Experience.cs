using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    [Required(ErrorMessage = "O ano de início é obrigatório.")] // Assuming StartYear is mandatory
    // Optional: Consider a [Range(1900, 2100, ErrorMessage = "Ano inválido")]
    public int StartYear { get; set; }

    // EndYear can be null if it's an ongoing experience
    // Optional: Consider validation like EndYear >= StartYear if EndYear is not null
    public int? EndYear { get; set; }  

    [Required(ErrorMessage = "A referência ao talento (TalentId) é obrigatória.")] // Make the Foreign Key required
    public int TalentId { get; set; }

    [ForeignKey("TalentId")]  
    public virtual Talent? Talent { get; set; } 
}
