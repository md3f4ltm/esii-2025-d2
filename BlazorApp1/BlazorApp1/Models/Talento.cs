using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESII2025d2.Models;

[Table("Talento")]
public partial class Talento
{
    [Key]
    public int id { get; set; }

    public string nome { get; set; } = null!;

    public string pais { get; set; } = null!;

    public string email { get; set; } = null!;

    public decimal precohora { get; set; }

    public int? codcategoriatalento { get; set; }

    public int idutilizador { get; set; }

    public virtual ICollection<Experiencia> Experiencia { get; set; } = new List<Experiencia>();

    public virtual ICollection<TalentoSkill> TalentoSkills { get; set; } = new List<TalentoSkill>();

    [ForeignKey("codcategoriatalento")]
    public virtual CategoriaTalento? codcategoriatalentoNavigation { get; set; }

    [ForeignKey("idutilizador")]
    public virtual Utilizador? idutilizadorNavigation { get; set; }
}
