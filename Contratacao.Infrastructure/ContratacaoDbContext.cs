using Microsoft.EntityFrameworkCore;
using Contratacao.Domain.Entities;
using Contratacao.Domain.ValueObjects;

namespace Contratacao.Infrastructure;

public class ContratacaoDbContext : DbContext
{
    public ContratacaoDbContext(DbContextOptions<ContratacaoDbContext> options)
        : base(options)
    {
    }

    public DbSet<ContratacaoEntity> Contratacoes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContratacaoEntity>(entity =>
        {
            // Força a tabela no schema public
            entity.ToTable("Contratacoes", "public");

            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).ValueGeneratedNever();
            entity.Property(c => c.PropostaId).IsRequired();
            entity.Property(c => c.DataContratacao).IsRequired();

            entity.Property(c => c.Status).HasConversion(
                status => status.Id,
                id => StatusProposta.FromId(id)
            ).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}