namespace SimulacaoCredito.Application.DTOs;

public class TelemetriaResponseDto
{
    public string DataReferencia { get; set; } = string.Empty; // formato "yyyy-MM-dd"
    public List<TelemetriaEndpointDto> ListaEndpoints { get; set; } = new();
}

public class TelemetriaEndpointDto
{
    public string NomeApi { get; set; } = string.Empty;
    public int QtdRequisicoes { get; set; }
    public int TempoMedio { get; set; } // em milissegundos
    public int TempoMinimo { get; set; } // em milissegundos
    public int TempoMaximo { get; set; } // em milissegundos
    public decimal PercentualSucesso { get; set; } // qtd de retorno 200 com relação ao total
}
