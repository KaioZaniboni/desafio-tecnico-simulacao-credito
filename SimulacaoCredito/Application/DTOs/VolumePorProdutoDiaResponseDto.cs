namespace SimulacaoCredito.Application.DTOs;

public class VolumePorProdutoDiaResponseDto
{
    public string DataReferencia { get; set; } = string.Empty; // formato "yyyy-MM-dd"
    public List<VolumeProdutoDto> Simulacoes { get; set; } = new();
}

public class VolumeProdutoDto
{
    public int CodigoProduto { get; set; }
    public string DescricaoProduto { get; set; } = string.Empty;
    public decimal TaxaMediaJuro { get; set; }
    public decimal ValorMedioPrestacao { get; set; }
    public decimal ValorTotalDesejado { get; set; }
    public decimal ValorTotalCredito { get; set; }
}
