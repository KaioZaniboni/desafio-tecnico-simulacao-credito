using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimulacaoCredito.Domain.Entities;

[Table("Parcela", Schema = "dbo")]
public class Parcela
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long SimulacaoId { get; set; }

    [Required]
    public TipoAmortizacao TipoAmortizacao { get; set; }

    [Required]
    public int Numero { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorAmortizacao { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorJuros { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorPrestacao { get; set; }

    // Navigation property
    [ForeignKey(nameof(SimulacaoId))]
    public virtual Simulacao Simulacao { get; set; } = null!;
}
