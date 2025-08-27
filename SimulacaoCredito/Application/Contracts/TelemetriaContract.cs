using Flunt.Notifications;
using Flunt.Validations;

namespace SimulacaoCredito.Application.Contracts;

public class TelemetriaContract : Contract<DateTime>
{
    public TelemetriaContract(DateTime dataReferencia)
    {
        Requires()
            .IsNotNull(dataReferencia, nameof(dataReferencia), "Data de referência é obrigatória")
            .IsLowerOrEqualsThan(dataReferencia.Date, DateTime.Now.Date, nameof(dataReferencia), "Data de referência não pode ser futura")
            .IsGreaterOrEqualsThan(dataReferencia.Date, DateTime.Now.Date.AddYears(-1), nameof(dataReferencia), "Data de referência não pode ser anterior a 1 ano");
    }
}
