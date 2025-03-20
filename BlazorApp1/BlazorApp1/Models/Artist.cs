using System;
using System.Collections.Generic;

namespace ESII2025d2.Models;

public partial class Artist
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Bio { get; set; } = null!;

    public DateTime BirthDate { get; set; }
}
