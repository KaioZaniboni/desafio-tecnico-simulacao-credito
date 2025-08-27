using Flunt.Notifications;
using SimulacaoCredito.Application.Contracts;

namespace SimulacaoCredito.Application.DTOs;

public class SimulacaoRequestDto : Notifiable<Notification>
{
    public decimal ValorDesejado { get; set; }
    public int Prazo { get; set; }

    public new bool IsValid()
    {
        var contract = new SimulacaoRequestContract(this);
        AddNotifications(contract);
        return base.IsValid;
    }
}
