using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimulacaoCredito.Domain.Entities;

[Table("Produto", Schema = "dbo")]
public class Produto
{
    [Key]
    [Column("CO_PRODUTO")]
    public int CodigoProduto { get; set; }
    
    [Required]
    [Column("NO_PRODUTO")]
    [MaxLength(200)]
    public string NomeProduto { get; set; } = string.Empty;
    
    [Required]
    [Column("PC_TAXA_JUROS", TypeName = "numeric(18,11)")]
    public decimal TaxaJuros { get; set; }
    
    [Required]
    [Column("NU_MINIMO_MESES")]
    public short MinimoMeses { get; set; }
    
    [Column("NU_MAXIMO_MESES")]
    public short? MaximoMeses { get; set; }
    
    [Required]
    [Column("VR_MINIMO", TypeName = "numeric(18,2)")]
    public decimal ValorMinimo { get; set; }
    
    [Column("VR_MAXIMO", TypeName = "numeric(18,2)")]
    public decimal? ValorMaximo { get; set; }
}
