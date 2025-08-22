using Microsoft.EntityFrameworkCore;
using SimulacaoCredito.Domain.Entities;

namespace SimulacaoCredito.Infrastructure.Data;

public class ProdutoDbContext : DbContext
{
    public ProdutoDbContext(DbContextOptions<ProdutoDbContext> options) : base(options)
    {
    }

    public DbSet<Produto> Produtos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade Produto (somente leitura)
        modelBuilder.Entity<Produto>(entity =>
        {
            entity.HasKey(e => e.CodigoProduto);
            entity.ToTable("Produto", "dbo");
        });
    }
}
