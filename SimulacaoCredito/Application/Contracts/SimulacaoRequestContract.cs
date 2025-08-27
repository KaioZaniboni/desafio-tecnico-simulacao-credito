using Flunt.Notifications;
using Flunt.Validations;
using SimulacaoCredito.Application.DTOs;

namespace SimulacaoCredito.Application.Contracts;

public class SimulacaoRequestContract : Contract<SimulacaoRequestDto>
{
    public SimulacaoRequestContract(SimulacaoRequestDto simulacao)
    {
        Requires()
            .IsNotNull(simulacao, nameof(simulacao), "Dados da simulação são obrigatórios")
            .IsGreaterThan(simulacao.ValorDesejado, 0, nameof(simulacao.ValorDesejado), "Valor desejado deve ser maior que zero")
            .IsLowerOrEqualsThan(simulacao.ValorDesejado, 10000000.00m, nameof(simulacao.ValorDesejado), "Valor desejado não pode exceder R$ 10.000.000,00")
            .IsGreaterThan(simulacao.Prazo, 0, nameof(simulacao.Prazo), "Prazo deve ser maior que zero")
            .IsLowerOrEqualsThan(simulacao.Prazo, 420, nameof(simulacao.Prazo), "Prazo não pode exceder 420 meses");

        // Validações de negócio específicas
        if (simulacao.ValorDesejado < 1000 && simulacao.Prazo > 120)
        {
            AddNotification(nameof(simulacao.Prazo), "Para valores abaixo de R$ 1.000,00, o prazo máximo é de 120 meses");
        }

        if (simulacao.ValorDesejado >= 1000000 && simulacao.Prazo < 12)
        {
            AddNotification(nameof(simulacao.Prazo), "Para valores acima de R$ 1.000.000,00, o prazo mínimo é de 12 meses");
        }
    }
}
