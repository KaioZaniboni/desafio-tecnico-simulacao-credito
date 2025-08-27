using Flunt.Notifications;
using Flunt.Validations;

namespace SimulacaoCredito.Application.Contracts;

public class PaginacaoContract : Contract<(int pagina, int tamanhoPagina)>
{
    public PaginacaoContract(int pagina, int tamanhoPagina)
    {
        Requires()
            .IsGreaterThan(pagina, 0, nameof(pagina), "Número da página deve ser maior que zero")
            .IsLowerOrEqualsThan(pagina, 10000, nameof(pagina), "Número da página não pode exceder 10.000")
            .IsGreaterThan(tamanhoPagina, 0, nameof(tamanhoPagina), "Tamanho da página deve ser maior que zero")
            .IsLowerOrEqualsThan(tamanhoPagina, 100, nameof(tamanhoPagina), "Tamanho da página não pode exceder 100 registros");
    }
}
