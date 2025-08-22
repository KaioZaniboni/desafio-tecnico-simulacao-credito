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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade Simulacao
        modelBuilder.Entity<Simulacao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.DataCriacao).HasDefaultValueSql("SYSDATETIMEOFFSET()");
            
            // Índices
            entity.HasIndex(e => e.DataCriacao).HasDatabaseName("IX_Simulacao_DataCriacao");
            entity.HasIndex(e => e.CodigoProduto).HasDatabaseName("IX_Simulacao_CodigoProduto");
        });

        // Configuração da entidade Parcela
        modelBuilder.Entity<Parcela>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            
            // Relacionamento
            entity.HasOne(e => e.Simulacao)
                  .WithMany(s => s.Parcelas)
                  .HasForeignKey(e => e.SimulacaoId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Índices
            entity.HasIndex(e => new { e.SimulacaoId, e.TipoAmortizacao })
                  .HasDatabaseName("IX_Parcela_SimulacaoId_TipoAmortizacao");
        });

        // Configuração da entidade TelemetriaRequisicao
        modelBuilder.Entity<TelemetriaRequisicao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            // Índices
            entity.HasIndex(e => e.DataHora).HasDatabaseName("IX_TelemetriaRequisicao_DataHora");
            entity.HasIndex(e => e.NomeApi).HasDatabaseName("IX_TelemetriaRequisicao_NomeApi");
        });
    }
}
