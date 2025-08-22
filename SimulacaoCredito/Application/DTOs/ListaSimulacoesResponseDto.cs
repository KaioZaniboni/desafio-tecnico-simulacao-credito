namespace SimulacaoCredito.Application.DTOs;

public class ListaSimulacoesResponseDto
{
    public int Pagina { get; set; }
    public int QtdRegistros { get; set; }
    public int QtdRegistrosPagina { get; set; }
    public List<SimulacaoResumoDto> Registros { get; set; } = new();
}

public class SimulacaoResumoDto
{
    public long IdSimulacao { get; set; }
    public decimal ValorDesejado { get; set; }
    public int Prazo { get; set; }
    public decimal ValorTotalParcelas { get; set; }
}
