using Microsoft.EntityFrameworkCore;
using SimulacaoCredito.Domain.Entities;

namespace SimulacaoCredito.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Simulacao> Simulacoes { get; set; }
    public DbSet<Parcela> Parcelas { get; set; }
    public DbSet<TelemetriaRequisicao> TelemetriaRequisicoes { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Simulacao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.DataCriacao).HasDefaultValueSql("SYSDATETIMEOFFSET()");

            entity.HasIndex(e => e.DataCriacao).HasDatabaseName("IX_Simulacao_DataCriacao");
            entity.HasIndex(e => e.CodigoProduto).HasDatabaseName("IX_Simulacao_CodigoProduto");
        });

        modelBuilder.Entity<Parcela>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(e => e.Simulacao)
                  .WithMany(s => s.Parcelas)
                  .HasForeignKey(e => e.SimulacaoId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.SimulacaoId, e.TipoAmortizacao })
                  .HasDatabaseName("IX_Parcela_SimulacaoId_TipoAmortizacao");
        });

        modelBuilder.Entity<TelemetriaRequisicao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasIndex(e => e.DataHora).HasDatabaseName("IX_TelemetriaRequisicao_DataHora");
            entity.HasIndex(e => e.NomeApi).HasDatabaseName("IX_TelemetriaRequisicao_NomeApi");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.DataCriacao).HasDefaultValueSql("SYSDATETIMEOFFSET()");

            // Índices para performance
            entity.HasIndex(e => e.Username).IsUnique().HasDatabaseName("IX_Usuario_Username");
            entity.HasIndex(e => e.Email).HasDatabaseName("IX_Usuario_Email");
            entity.HasIndex(e => e.Ativo).HasDatabaseName("IX_Usuario_Ativo");
            entity.HasIndex(e => e.DataCriacao).HasDatabaseName("IX_Usuario_DataCriacao");

            // Configurações de propriedades
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.NomeCompleto).HasMaxLength(100);
        });
    }
}
