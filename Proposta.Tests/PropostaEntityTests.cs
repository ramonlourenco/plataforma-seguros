using Proposta.Domain.Entities;
using Proposta.Domain.Enums;
using Xunit;

namespace Proposta.Tests;

public class PropostaEntityTests
{
    [Fact(DisplayName = "Constructor with cliente nome and valor initializes correctly")]
    public void Constructor_WithClienteNomeAndValor_ShouldInitializeCorrectly()
    {
        // Arrange
        var clienteNome = "João Silva";
        var valor = 1500m;

        // Act
        var proposta = new PropostaEntity(clienteNome, valor);

        // Assert
        Assert.NotEqual(Guid.Empty, proposta.Id);
        Assert.Equal(clienteNome, proposta.ClienteNome);
        Assert.Equal(valor, proposta.Valor);
        Assert.Equal(PropostaStatus.EmAnalise, proposta.Status);
        Assert.NotEqual(DateTime.MinValue, proposta.CriadaEm);
        Assert.Equal(DateTimeKind.Utc, proposta.CriadaEm.Kind);
    }

    [Fact(DisplayName = "Aprovar should change status to Aprovada")]
    public void Aprovar_ShouldChangeStatusToAprovada()
    {
        // Arrange
        var proposta = new PropostaEntity("Cliente", 1000m);
        Assert.Equal(PropostaStatus.EmAnalise, proposta.Status);

        // Act
        proposta.Aprovar();

        // Assert
        Assert.Equal(PropostaStatus.Aprovada, proposta.Status);
    }

    [Fact(DisplayName = "Rejeitar should change status to Rejeitada")]
    public void Rejeitar_ShouldChangeStatusToRejeitada()
    {
        // Arrange
        var proposta = new PropostaEntity("Cliente", 1000m);
        Assert.Equal(PropostaStatus.EmAnalise, proposta.Status);

        // Act
        proposta.Rejeitar();

        // Assert
        Assert.Equal(PropostaStatus.Rejeitada, proposta.Status);
    }

    [Fact(DisplayName = "Aprovar from Rejeitada status should change status to Aprovada")]
    public void Aprovar_FromRejeitadaStatus_ShouldChangeStatusToAprovada()
    {
        // Arrange
        var proposta = new PropostaEntity("Cliente", 1000m);
        proposta.Rejeitar();
        Assert.Equal(PropostaStatus.Rejeitada, proposta.Status);

        // Act
        proposta.Aprovar();

        // Assert
        Assert.Equal(PropostaStatus.Aprovada, proposta.Status);
    }

    [Fact(DisplayName = "Rejeitar from Aprovada status should change status to Rejeitada")]
    public void Rejeitar_FromAprovadaStatus_ShouldChangeStatusToRejeitada()
    {
        // Arrange
        var proposta = new PropostaEntity("Cliente", 1000m);
        proposta.Aprovar();
        Assert.Equal(PropostaStatus.Aprovada, proposta.Status);

        // Act
        proposta.Rejeitar();

        // Assert
        Assert.Equal(PropostaStatus.Rejeitada, proposta.Status);
    }
}
