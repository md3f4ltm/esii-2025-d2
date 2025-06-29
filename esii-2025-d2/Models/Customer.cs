using esii_2025_d2.Data; 
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace esii_2025_d2.Models;

public class Customer
{
    [Key]
    public string Id { get; set; } = null!;

    [Required]
    [StringLength(200)]
    public string Company { get; set; } = null!;

    [Required]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = null!; // Foreign key property

    [JsonIgnore]
    [ForeignKey("UserId")]
    public virtual ApplicationUser? User { get; set; }

    public virtual ICollection<JobProposal> JobProposals { get; set; } = new List<JobProposal>();
}
