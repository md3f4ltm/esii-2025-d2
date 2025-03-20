using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESII2025d2.Models;

[Table("CategoriaTalento")]
public partial class CategoriaTalento
{
    [Key]
    public int cod { get; set; }

    public string nome { get; set; } = null!;

    public virtual ICollection<PropostaTrabalho> PropostaTrabalhos { get; set; } = new List<PropostaTrabalho>();

    public virtual ICollection<Talento> Talentos { get; set; } = new List<Talento>();
}
