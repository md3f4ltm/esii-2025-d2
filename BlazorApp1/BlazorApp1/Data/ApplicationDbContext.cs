using BlazorApp1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure composite primary key for TalentoSkill
            builder.Entity<TalentoSkill>()
                .HasKey(ts => new { ts.IdTalento, ts.CodSkill });

            // Configure relationships
            builder.Entity<TalentoSkill>()
                .HasOne(ts => ts.Skill)
                .WithMany(s => s.TalentoSkills)
                .HasForeignKey(ts => ts.CodSkill);

            builder.Entity<TalentoSkill>()
                .HasOne(ts => ts.Talento)
                .WithMany(t => t.TalentoSkills)
                .HasForeignKey(ts => ts.IdTalento);

            // Configure unique constraint for PropostaTrabalho
            builder.Entity<PropostaTrabalho>()
                .HasIndex(pt => new { pt.CodSkill, pt.CodTalento })
                .IsUnique();

            // User relationship configurations
            builder.Entity<Cliente>()
                .HasOne(c => c.User)
                .WithOne(u => u.Cliente)
                .HasForeignKey<Cliente>(c => c.UserId);

            builder.Entity<Talento>()
                .HasOne(t => t.User)
                .WithOne(u => u.Talento)
                .HasForeignKey<Talento>(t => t.UserId);
        }

        // DbSet for each entity
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<CategoriaTalento> CategoriasTalento { get; set; }
        public DbSet<Talento> Talentos { get; set; }
        public DbSet<Experiencia> Experiencias { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<TalentoSkill> TalentoSkills { get; set; }
        public DbSet<PropostaTrabalho> PropostasTrabalho { get; set; }
    }
}
