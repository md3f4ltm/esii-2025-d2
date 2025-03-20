using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESII2025d2.Models;

[Table("Experiencia")]
public partial class Experiencia
{
    [Key]
    public int id { get; set; }

    public string titulo { get; set; } = null!;

    public string nomeempresa { get; set; } = null!;

    public int anocomeco { get; set; }

    public int? anofim { get; set; }

    public int idtalento { get; set; }

    [ForeignKey("idtalento")]
    public virtual Talento idtalentoNavigation { get; set; } = null!;
}
