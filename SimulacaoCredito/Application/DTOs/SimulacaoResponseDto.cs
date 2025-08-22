namespace SimulacaoCredito.Application.DTOs;

public class SimulacaoResponseDto
{
    public long IdSimulacao { get; set; }
    public int CodigoProduto { get; set; }
    public string DescricaoProduto { get; set; } = string.Empty;
    public decimal TaxaJuros { get; set; }
    public List<ResultadoAmortizacaoDto> ResultadoSimulacao { get; set; } = new();
}
