using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimulacaoCredito.Domain.Entities;

[Table("Simulacao", Schema = "dbo")]
public class Simulacao
{
    [Key]
    public long Id { get; set; }

    [Required]
    public DateTimeOffset DataCriacao { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorDesejado { get; set; }

    [Required]
    public int Prazo { get; set; }

    [Required]
    public int CodigoProduto { get; set; }

    [Required]
    [MaxLength(200)]
    public string DescricaoProduto { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(9,6)")]
    public decimal TaxaJuros { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorTotalParcelas { get; set; }

    // Navigation property
    public virtual ICollection<Parcela> Parcelas { get; set; } = new List<Parcela>();
}
