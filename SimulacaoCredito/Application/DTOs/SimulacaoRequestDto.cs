using System.ComponentModel.DataAnnotations;

namespace SimulacaoCredito.Application.DTOs;

public class SimulacaoRequestDto
{
    [Required(ErrorMessage = "Valor desejado é obrigatório")]
    [Range(0.01, 10000000.00, ErrorMessage = "Valor desejado deve estar entre R$ 0,01 e R$ 10.000.000,00")]
    public decimal ValorDesejado { get; set; }

    [Required(ErrorMessage = "Prazo é obrigatório")]
    [Range(1, 480, ErrorMessage = "Prazo deve estar entre 1 e 480 meses")]
    public int Prazo { get; set; }
}
