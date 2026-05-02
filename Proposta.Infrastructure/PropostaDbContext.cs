using Microsoft.EntityFrameworkCore;
using Proposta.Domain.Entities;
using Proposta.Domain.Enums;

namespace Proposta.Infrastructure;

public class PropostaDbContext : DbContext
{
    public PropostaDbContext(DbContextOptions<PropostaDbContext> options) : base(options) { }

    public DbSet<PropostaEntity> Propostas { get; set; } = null!;
    public DbSet<PropostaStatusEntity> StatusPropostas { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");

        modelBuilder.Entity<PropostaStatusEntity>(entity =>
        {
            entity.ToTable("PropostaStatus");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).ValueGeneratedNever();
            entity.Property(s => s.Nome).IsRequired().HasMaxLength(50);

            entity.HasData(
                new PropostaStatusEntity(1, "EmAnalise"),
                new PropostaStatusEntity(2, "Aprovada"),
                new PropostaStatusEntity(3, "Rejeitada")
            );
        });

        modelBuilder.Entity<PropostaEntity>(entity =>
        {
            entity.ToTable("Propostas");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).ValueGeneratedNever();

            entity.HasOne(p => p.StatusReference)
                .WithMany()
                .HasForeignKey(p => p.StatusId)
                .IsRequired();

            entity.Ignore(p => p.Status);
        });

        modelBuilder.Entity<PropostaEntity>().HasData(
            new { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), ClienteNome = "João Silva", Valor = 1200m, StatusId = 1, CriadaEm = DateTime.UtcNow },
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), ClienteNome = "Maria Oliveira", Valor = 2300m, StatusId = 2, CriadaEm = DateTime.UtcNow },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), ClienteNome = "Ana Luiza", Valor = 2300m, StatusId = 3, CriadaEm = DateTime.UtcNow }
        );

        base.OnModelCreating(modelBuilder);
    }
}