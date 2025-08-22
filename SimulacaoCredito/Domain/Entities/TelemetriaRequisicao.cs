using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimulacaoCredito.Domain.Entities;

[Table("TelemetriaRequisicao", Schema = "dbo")]
public class TelemetriaRequisicao
{
    [Key]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string NomeApi { get; set; } = string.Empty;

    [Required]
    public DateTime DataHora { get; set; }

    [Required]
    public int TempoResposta { get; set; } // em milissegundos

    [Required]
    public bool Sucesso { get; set; }

    [Required]
    public int StatusCode { get; set; }

    [MaxLength(500)]
    public string? Erro { get; set; }

    [MaxLength(50)]
    public string? UsuarioId { get; set; }

    [MaxLength(45)]
    public string? EnderecoIp { get; set; }
}
