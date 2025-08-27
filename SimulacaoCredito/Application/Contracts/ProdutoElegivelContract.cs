using Flunt.Notifications;
using Flunt.Validations;

namespace SimulacaoCredito.Application.Contracts;

public class ProdutoElegivelContract : Contract<(decimal valor, int prazo)>
{
    public ProdutoElegivelContract(decimal valor, int prazo)
    {
        Requires()
            .IsGreaterThan(valor, 0, nameof(valor), "Valor deve ser maior que zero")
            .IsLowerOrEqualsThan(valor, 10000000.00m, nameof(valor), "Valor não pode exceder R$ 10.000.000,00")
            .IsGreaterThan(prazo, 0, nameof(prazo), "Prazo deve ser maior que zero")
            .IsLowerOrEqualsThan(prazo, 420, nameof(prazo), "Prazo não pode exceder 420 meses");
    }
}
