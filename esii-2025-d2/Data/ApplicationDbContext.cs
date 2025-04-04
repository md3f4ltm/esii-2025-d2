// esii-2025-d2/Data/ApplicationDbContext.cs
using esii_2025_d2.Models; // Your models namespace
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace esii_2025_d2.Data;

// *** Change ApplicationUser to User here ***
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    // --- Add DbSets for all your models ---
    public DbSet<Customer> Customers { get; set; }
    public DbSet<TalentCategory> TalentCategories { get; set; }
    public DbSet<Experience> Experiences { get; set; }
    public DbSet<JobProposal> JobProposals { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Talent> Talents { get; set; }
    public DbSet<TalentSkill> TalentSkills { get; set; }

    // No need for DbSet<User> Users - Handled by IdentityDbContext<User>

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // *** IMPORTANT: Call base.OnModelCreating FIRST ***
        base.OnModelCreating(modelBuilder);

        // --- Configure the composite key for TalentSkill ---
        modelBuilder.Entity<TalentSkill>()
            .HasKey(ts => new { ts.TalentId, ts.SkillId }); // Use the English property names

        // --- Add any other custom configurations needed ---
        // Example: If you need specific table names (though convention is preferred)
        // modelBuilder.Entity<Customer>().ToTable("Cliente");

        // Example: Configure decimal precision
        modelBuilder.Entity<Talent>()
            .Property(t => t.HourlyRate)
            .HasColumnType("decimal(18, 2)");

         // Example: Define relationships explicitly if needed (often covered by conventions)
         modelBuilder.Entity<TalentSkill>()
             .HasOne(ts => ts.Talent)
             .WithMany(t => t.TalentSkills)
             .HasForeignKey(ts => ts.TalentId);

         modelBuilder.Entity<TalentSkill>()
             .HasOne(ts => ts.Skill)
             .WithMany(s => s.TalentSkills)
             .HasForeignKey(ts => ts.SkillId);
    }
}
