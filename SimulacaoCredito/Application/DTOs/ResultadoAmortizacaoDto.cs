namespace SimulacaoCredito.Application.DTOs;

public class ResultadoAmortizacaoDto
{
    public string Tipo { get; set; } = string.Empty; // "SAC" ou "PRICE"
    public List<ParcelaDto> Parcelas { get; set; } = new();
}
