using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using esii_2025_d2.Models;

namespace esii_2025_d2.Data
{
    public class ApplicationUser : IdentityUser
    {
        // Propriedades do Perfil que já estavam ou foram adicionadas
        public string? Description { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Area { get; set; }
        
        // Alteração de 'List' para 'virtual ICollection' (boa prática, não remove funcionalidade)
        public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();

        [StringLength(100)]
        public string? Name { get; set; }


        [DataType(DataType.Date)]
        [Display(Name="Date of Birth")]
        public DateOnly? DateOfBirth { get; set; } 

        // As suas coleções foram mantidas como pediu.
        public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
        public virtual ICollection<Talent> Talents { get; set; } = new List<Talent>();
    }
}