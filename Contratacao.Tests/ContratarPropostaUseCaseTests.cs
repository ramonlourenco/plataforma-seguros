using Moq;
using Contratacao.Application.UseCases;
using Contratacao.Domain.Entities;
using Contratacao.Domain.Ports;
using Contratacao.Domain.ValueObjects;
using Xunit;

namespace Contratacao.Tests;

public class ContratarPropostaUseCaseTests
{
    private readonly Mock<IPropostaServiceClient> _propostaClientMock;
    private readonly Mock<IContratacaoRepository> _contratacaoRepositoryMock;
    private readonly ContratarPropostaUseCase _useCase;

    public ContratarPropostaUseCaseTests()
    {
        _propostaClientMock = new Mock<IPropostaServiceClient>();
        _contratacaoRepositoryMock = new Mock<IContratacaoRepository>();
        _useCase = new ContratarPropostaUseCase(_propostaClientMock.Object, _contratacaoRepositoryMock.Object);
    }

    [Fact(DisplayName = "ExecuteAsync when proposta aprovada returns contratacao entity")]
    public async Task ExecuteAsync_WhenPropostaIsAprovada_ReturnsContratacaoEntity()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var proposta = new Proposta(propostaId, "Cliente", 1500m, StatusProposta.Aprovada);
        var contratacao = new ContratacaoEntity(propostaId);

        _propostaClientMock
            .Setup(x => x.GetPropostaByIdAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(proposta);

        _contratacaoRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<ContratacaoEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contratacao);

        // Act
        var result = await _useCase.ExecuteAsync(propostaId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(propostaId, result!.PropostaId);
        Assert.Equal(StatusProposta.Contratada, result.Status);
    }

    [Fact(DisplayName = "ExecuteAsync when proposta em analise returns null")]
    public async Task ExecuteAsync_WhenPropostaIsEmAnalise_ReturnsNull()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var proposta = new Proposta(propostaId, "Cliente", 1500m, StatusProposta.EmAnalise);

        _propostaClientMock
            .Setup(x => x.GetPropostaByIdAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(proposta);

        // Act
        var result = await _useCase.ExecuteAsync(propostaId);

        // Assert
        Assert.Null(result);
    }

    [Fact(DisplayName = "ExecuteAsync when proposta not aprovada returns null")]
    public async Task ExecuteAsync_WhenPropostaIsNotAprovada_ReturnsNull()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var proposta = new Proposta(propostaId, "Cliente", 1500m, StatusProposta.Rejeitada);

        _propostaClientMock
            .Setup(x => x.GetPropostaByIdAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(proposta);

        // Act
        var result = await _useCase.ExecuteAsync(propostaId);

        // Assert
        Assert.Null(result);
    }

    [Fact(DisplayName = "ExecuteAsync when proposta not found returns null")]
    public async Task ExecuteAsync_WhenPropostaNotFound_ReturnsNull()
    {
        // Arrange
        var propostaId = Guid.NewGuid();

        _propostaClientMock
            .Setup(x => x.GetPropostaByIdAsync(propostaId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new System.Net.Http.HttpRequestException("Response status code does not indicate success: 404 (Not Found)."));

        // Act
        var result = await _useCase.ExecuteAsync(propostaId);

        // Assert
        Assert.Null(result);
    }
}
