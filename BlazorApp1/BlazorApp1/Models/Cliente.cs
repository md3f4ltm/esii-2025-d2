using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ESII2025d2.Models;

[Table("Cliente")]
public partial class Cliente
{
    [Key]

    public string id { get; set; }
    public string empresa { get; set; } = null!;

    public string numerotelefone { get; set; } = null!;

    public string idutilizador { get; set; } = null!;

    public virtual ICollection<PropostaTrabalho> PropostaTrabalhos { get; set; } = new List<PropostaTrabalho>();

    [JsonIgnore]
    [ForeignKey("idutilizador")]
    public virtual Utilizador? idutilizadorNavigation { get; set; }
}
