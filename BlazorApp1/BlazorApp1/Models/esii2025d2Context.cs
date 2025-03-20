using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ESII2025d2.Models;

public partial class esii2025d2Context : DbContext
{
    public esii2025d2Context()
    {
    }

    public esii2025d2Context(DbContextOptions<esii2025d2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Artist> Artists { get; set; }

    public virtual DbSet<CategoriaTalento> CategoriaTalentos { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Experiencia> Experiencia { get; set; }

    public virtual DbSet<PropostaTrabalho> PropostaTrabalhos { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<Talento> Talentos { get; set; }

    public virtual DbSet<TalentoSkill> TalentoSkills { get; set; }

    public virtual DbSet<Utilizador> Utilizadors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=esii-2025-d2;Username=postgres;Password=123456");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CategoriaTalento>(entity =>
        {
            entity.HasKey(e => e.cod).HasName("categoriatalento_pkey");

            entity.ToTable("CategoriaTalento");

            entity.Property(e => e.cod).HasDefaultValueSql("nextval('categoriatalento_cod_seq'::regclass)");
            entity.Property(e => e.nome).HasMaxLength(255);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.id).HasName("cliente_pkey");

            entity.ToTable("Cliente");

            entity.Property(e => e.id).HasDefaultValueSql("nextval('cliente_id_seq'::regclass)");
            entity.Property(e => e.empresa).HasMaxLength(255);
            entity.Property(e => e.numerotelefone).HasMaxLength(20);

            entity.HasOne(d => d.idutilizadorNavigation).WithMany(p => p.Clientes)
                .HasForeignKey(d => d.idutilizador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cliente_idutilizador_fkey");
        });

        modelBuilder.Entity<Experiencia>(entity =>
        {
            entity.HasKey(e => e.id).HasName("experiencia_pkey");

            entity.Property(e => e.id).HasDefaultValueSql("nextval('experiencia_id_seq'::regclass)");
            entity.Property(e => e.nomeempresa).HasMaxLength(255);
            entity.Property(e => e.titulo).HasMaxLength(255);

            entity.HasOne(d => d.idtalentoNavigation).WithMany(p => p.Experiencia)
                .HasForeignKey(d => d.idtalento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("experiencia_idtalento_fkey");
        });

        modelBuilder.Entity<PropostaTrabalho>(entity =>
        {
            entity.HasKey(e => e.cod).HasName("propostatrabalho_pkey");

            entity.ToTable("PropostaTrabalho");

            entity.HasIndex(e => e.cattalento_cod, "fki_cattalento_cod_fk");

            entity.HasIndex(e => e.cliente_id, "fki_cliente_id_fk");

            entity.Property(e => e.cod).HasDefaultValueSql("nextval('propostatrabalho_cod_seq'::regclass)");
            entity.Property(e => e.nome).HasMaxLength(255);

            entity.HasOne(d => d.cattalento_codNavigation).WithMany(p => p.PropostaTrabalhos)
                .HasForeignKey(d => d.cattalento_cod)
                .HasConstraintName("cattalento_cod_fk");

            entity.HasOne(d => d.cliente).WithMany(p => p.PropostaTrabalhos)
                .HasForeignKey(d => d.cliente_id)
                .HasConstraintName("cliente_id_fk");

            entity.HasOne(d => d.codskillNavigation).WithMany(p => p.PropostaTrabalhos)
                .HasForeignKey(d => d.codskill)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("propostatrabalho_codskill_fkey");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.cod).HasName("skill_pkey");

            entity.ToTable("Skill");

            entity.Property(e => e.cod).HasDefaultValueSql("nextval('skill_cod_seq'::regclass)");
            entity.Property(e => e.area).HasMaxLength(255);
            entity.Property(e => e.nome).HasMaxLength(255);
        });

        modelBuilder.Entity<Talento>(entity =>
        {
            entity.HasKey(e => e.id).HasName("talento_pkey");

            entity.ToTable("Talento");

            entity.HasIndex(e => e.email, "talento_email_key").IsUnique();

            entity.Property(e => e.id).HasDefaultValueSql("nextval('talento_id_seq'::regclass)");
            entity.Property(e => e.email).HasMaxLength(255);
            entity.Property(e => e.nome).HasMaxLength(255);
            entity.Property(e => e.pais).HasMaxLength(100);
            entity.Property(e => e.precohora).HasPrecision(10, 2);

            entity.HasOne(d => d.codcategoriatalentoNavigation).WithMany(p => p.Talentos)
                .HasForeignKey(d => d.codcategoriatalento)
                .HasConstraintName("talento_codcategoriatalento_fkey");

            entity.HasOne(d => d.idutilizadorNavigation).WithMany(p => p.Talentos)
                .HasForeignKey(d => d.idutilizador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("talento_idutilizador_fkey");
        });

        modelBuilder.Entity<TalentoSkill>(entity =>
        {
            entity.HasKey(e => new { e.codskill, e.idtalento }).HasName("talento_skill_pkey");

            entity.ToTable("TalentoSkill");

            entity.HasOne(d => d.codskillNavigation).WithMany(p => p.TalentoSkills)
                .HasForeignKey(d => d.codskill)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("talento_skill_codskill_fkey");

            entity.HasOne(d => d.idtalentoNavigation).WithMany(p => p.TalentoSkills)
                .HasForeignKey(d => d.idtalento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("talento_skill_idtalento_fkey");
        });

        modelBuilder.Entity<Utilizador>(entity =>
        {
            entity.HasKey(e => e.id).HasName("utilizador_pkey");

            entity.ToTable("Utilizador");

            entity.HasIndex(e => e.email, "utilizador_email_key").IsUnique();

            entity.HasIndex(e => e.username, "utilizador_username_key").IsUnique();

            entity.Property(e => e.id).HasDefaultValueSql("nextval('utilizador_id_seq'::regclass)");
            entity.Property(e => e.email).HasMaxLength(255);
            entity.Property(e => e.nome).HasMaxLength(255);
            entity.Property(e => e.palavra_passe)
                .HasMaxLength(255)
                .HasColumnName("palavra-passe");
            entity.Property(e => e.username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
