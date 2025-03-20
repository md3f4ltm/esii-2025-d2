using ESII2025d2.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Cliente> Clientes { get; set; }
    
    public DbSet<CategoriaTalento> CategoriasTalento { get; set; }
    
    public DbSet<Experiencia> Experiencias { get; set; }
    public DbSet<PropostaTrabalho> PropostaTrabalhos { get; set; }
    public DbSet<Skill> Skills { get; set; }
    
    public DbSet<Talento> Talentos { get; set; }
    public DbSet<TalentoSkill> TalentoSkills { get; set; }
    public DbSet<Utilizador> Utilizadores { get; set; }
    
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        

    }
}