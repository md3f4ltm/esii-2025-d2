using ESII2025d2.Models; // Your models namespace (Ensure this is correct)
using Microsoft.AspNetCore.Identity; // Base Identity types (might be needed)
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // REQUIRED for IdentityDbContext
using Microsoft.EntityFrameworkCore; // REQUIRED for DbContext, DbContextOptions etc.

public class ApplicationDbContext : IdentityDbContext<Utilizador>
{
    // Constructor should accept DbContextOptions<ApplicationDbContext>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) // Pass options to the base constructor
    {
    }

    // DbSets for your other models
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<CategoriaTalento> CategoriasTalento { get; set; }
    public DbSet<Experiencia> Experiencias { get; set; }
    public DbSet<PropostaTrabalho> PropostaTrabalhos { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Talento> Talentos { get; set; }
    public DbSet<TalentoSkill> TalentoSkills { get; set; }

    // No need to declare DbSet<Utilizador> Utilizadores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // MUST call base.OnModelCreating FIRST for Identity
        base.OnModelCreating(modelBuilder);

        // Add any additional custom configurations for your models here
    }
}
