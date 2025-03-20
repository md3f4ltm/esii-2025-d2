using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESII2025d2.Models;

[Table("Skill")]
public partial class Skill
{
    [Key]
    public int cod { get; set; }

    public string nome { get; set; } = null!;

    public string? area { get; set; }

    public virtual ICollection<PropostaTrabalho> PropostaTrabalhos { get; set; } = new List<PropostaTrabalho>();

    public virtual ICollection<TalentoSkill> TalentoSkills { get; set; } = new List<TalentoSkill>();
}
