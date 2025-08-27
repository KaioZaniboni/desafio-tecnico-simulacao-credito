using SimulacaoCredito.Application.DTOs;
using SimulacaoCredito.Application.Contracts;
using Xunit;

namespace SimulacaoCredito.Tests;

public class FluntValidationTests
{
    [Fact]
    public void SimulacaoRequestDto_ValidData_ShouldBeValid()
    {
        // Arrange
        var request = new SimulacaoRequestDto
        {
            ValorDesejado = 50000m,
            Prazo = 24
        };

        // Act
        var isValid = request.IsValid();

        // Assert
        Assert.True(isValid);
        Assert.Empty(request.Notifications);
    }

    [Fact]
    public void SimulacaoRequestDto_InvalidValorDesejado_ShouldBeInvalid()
    {
        // Arrange
        var request = new SimulacaoRequestDto
        {
            ValorDesejado = 0m,
            Prazo = 24
        };

        // Act
        var isValid = request.IsValid();

        // Assert
        Assert.False(isValid);
        Assert.Contains(request.Notifications, n => n.Key == nameof(request.ValorDesejado));
    }

    [Fact]
    public void SimulacaoRequestDto_InvalidPrazo_ShouldBeInvalid()
    {
        // Arrange
        var request = new SimulacaoRequestDto
        {
            ValorDesejado = 50000m,
            Prazo = 0
        };

        // Act
        var isValid = request.IsValid();

        // Assert
        Assert.False(isValid);
        Assert.Contains(request.Notifications, n => n.Key == nameof(request.Prazo));
    }

    [Fact]
    public void SimulacaoRequestDto_BusinessRule_LowValueHighPeriod_ShouldBeInvalid()
    {
        // Arrange
        var request = new SimulacaoRequestDto
        {
            ValorDesejado = 500m, // Valor baixo
            Prazo = 150 // Prazo alto
        };

        // Act
        var isValid = request.IsValid();

        // Assert
        Assert.False(isValid);
        Assert.Contains(request.Notifications, n => n.Key == nameof(request.Prazo) && 
            n.Message.Contains("Para valores abaixo de R$ 1.000,00"));
    }

    [Fact]
    public void SimulacaoRequestDto_BusinessRule_HighValueLowPeriod_ShouldBeInvalid()
    {
        // Arrange
        var request = new SimulacaoRequestDto
        {
            ValorDesejado = 2000000m, // Valor alto
            Prazo = 6 // Prazo baixo
        };

        // Act
        var isValid = request.IsValid();

        // Assert
        Assert.False(isValid);
        Assert.Contains(request.Notifications, n => n.Key == nameof(request.Prazo) &&
            n.Message.Contains("Para valores acima de R$ 1.000.000,00"));
    }

    [Fact]
    public void SimulacaoRequestDto_MaxPrazo420_ShouldBeValid()
    {
        // Arrange
        var request = new SimulacaoRequestDto
        {
            ValorDesejado = 50000m,
            Prazo = 420 // Prazo máximo permitido
        };

        // Act
        var isValid = request.IsValid();

        // Assert
        Assert.True(isValid);
        Assert.Empty(request.Notifications);
    }

    [Fact]
    public void SimulacaoRequestDto_PrazoAbove420_ShouldBeInvalid()
    {
        // Arrange
        var request = new SimulacaoRequestDto
        {
            ValorDesejado = 50000m,
            Prazo = 421 // Prazo acima do máximo
        };

        // Act
        var isValid = request.IsValid();

        // Assert
        Assert.False(isValid);
        Assert.Contains(request.Notifications, n => n.Key == nameof(request.Prazo) &&
            n.Message.Contains("não pode exceder 420 meses"));
    }

    [Fact]
    public void ProdutoElegivelContract_ValidData_ShouldBeValid()
    {
        // Arrange
        var contract = new ProdutoElegivelContract(50000m, 24);

        // Act & Assert
        Assert.True(contract.IsValid);
        Assert.Empty(contract.Notifications);
    }

    [Fact]
    public void ProdutoElegivelContract_InvalidData_ShouldBeInvalid()
    {
        // Arrange
        var contract = new ProdutoElegivelContract(-1000m, 0);

        // Act & Assert
        Assert.False(contract.IsValid);
        Assert.NotEmpty(contract.Notifications);
    }

    [Fact]
    public void PaginacaoContract_ValidData_ShouldBeValid()
    {
        // Arrange
        var contract = new PaginacaoContract(1, 10);

        // Act & Assert
        Assert.True(contract.IsValid);
        Assert.Empty(contract.Notifications);
    }

    [Fact]
    public void PaginacaoContract_InvalidData_ShouldBeInvalid()
    {
        // Arrange
        var contract = new PaginacaoContract(0, 101);

        // Act & Assert
        Assert.False(contract.IsValid);
        Assert.NotEmpty(contract.Notifications);
    }

    [Fact]
    public void TelemetriaContract_ValidData_ShouldBeValid()
    {
        // Arrange
        var dataReferencia = DateTime.Now.Date.AddDays(-1);
        var contract = new TelemetriaContract(dataReferencia);

        // Act & Assert
        Assert.True(contract.IsValid);
        Assert.Empty(contract.Notifications);
    }

    [Fact]
    public void TelemetriaContract_FutureDate_ShouldBeInvalid()
    {
        // Arrange
        var dataReferencia = DateTime.Now.Date.AddDays(1);
        var contract = new TelemetriaContract(dataReferencia);

        // Act & Assert
        Assert.False(contract.IsValid);
        Assert.Contains(contract.Notifications, n => n.Message.Contains("não pode ser futura"));
    }
}
