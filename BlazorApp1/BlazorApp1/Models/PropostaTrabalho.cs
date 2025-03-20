using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESII2025d2.Models;

[Table("PropostaTrabalho")]
public partial class PropostaTrabalho
{
    [Key]
    public int cod { get; set; }

    public int codskill { get; set; }

    public string nome { get; set; } = null!;

    public int numtotalhoras { get; set; }

    public string? descricao { get; set; }

    public int? cliente_id { get; set; }

    public int? cattalento_cod { get; set; }

    [ForeignKey("cattalento_cod")]
    public virtual CategoriaTalento? cattalento_codNavigation { get; set; }

    [ForeignKey("cliente_id")]
    public virtual Cliente? cliente { get; set; }

    [ForeignKey("codskill")]
    public virtual Skill codskillNavigation { get; set; } = null!;
}
