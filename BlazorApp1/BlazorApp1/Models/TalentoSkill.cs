using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESII2025d2.Models;

[Table("TalentoSkill")]
public partial class TalentoSkill
{
    [Key]
    public int codskill { get; set; }

    public int idtalento { get; set; }

    public int anosexperiencia { get; set; }

    [ForeignKey("codskill")]
    public virtual Skill codskillNavigation { get; set; } = null!;
    
    [ForeignKey("idtalento")]
    public virtual Talento idtalentoNavigation { get; set; } = null!;
}
