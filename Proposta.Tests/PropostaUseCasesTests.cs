using System.Linq;
using Moq;
using Proposta.Application.UseCases;
using Proposta.Domain.Entities;
using Proposta.Domain.Enums;
using Proposta.Domain.Ports;
using Xunit;

namespace Proposta.Tests;

public class PropostaUseCasesTests
{
    private readonly Mock<IPropostaRepository> _repositoryMock;
    private readonly PropostaUseCases _useCase;

    public PropostaUseCasesTests()
    {
        _repositoryMock = new Mock<IPropostaRepository>();
        _useCase = new PropostaUseCases(_repositoryMock.Object);
    }

    #region CreateAsync

    [Fact(DisplayName = "CreateAsync with valid data should create and return entity")]
    public async Task CreateAsync_WithValidData_ShouldCreateAndReturn()
    {
        // Arrange
        var clienteNome = "João Silva";
        var valor = 1000m;
        PropostaEntity? capturedEntity = null;

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<PropostaEntity>(), It.IsAny<CancellationToken>()))
            .Callback<PropostaEntity, CancellationToken>((entity, _) => capturedEntity = entity)
            .ReturnsAsync((PropostaEntity entity, CancellationToken _) => entity);

        // Act
        var result = await _useCase.CreateAsync(clienteNome, valor);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(clienteNome, result.ClienteNome);
        Assert.Equal(valor, result.Valor);
        Assert.Equal(PropostaStatus.EmAnalise, result.Status);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<PropostaEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetByIdAsync

    [Fact(DisplayName = "GetByIdAsync with existing id should return proposta")]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnProposta()
    {
        // Arrange
        var id = Guid.NewGuid();
        var proposta = new PropostaEntity("Maria", 2000m);
        proposta.Aprovar();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(proposta);

        // Act
        var result = await _useCase.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(proposta.Id, result.Id);
        Assert.Equal("Maria", result.ClienteNome);
        Assert.Equal(PropostaStatus.Aprovada, result.Status);
        _repositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "GetByIdAsync with non-existing id should return null")]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropostaEntity?)null);

        // Act
        var result = await _useCase.GetByIdAsync(id);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetAllAsync

    [Fact(DisplayName = "GetAllAsync with propostas should return list")]
    public async Task GetAllAsync_WithPropostas_ShouldReturnList()
    {
        // Arrange
        var propostas = new List<PropostaEntity>
        {
            new PropostaEntity("Cliente 1", 1000m),
            new PropostaEntity("Cliente 2", 2000m)
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(propostas);

        // Act
        var result = await _useCase.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Equal("Cliente 1", resultList[0].ClienteNome);
        Assert.Equal("Cliente 2", resultList[1].ClienteNome);
        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "GetAllAsync with empty repository should return empty list")]
    public async Task GetAllAsync_WithEmptyRepository_ShouldReturnEmptyList()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PropostaEntity>());

        // Act
        var result = await _useCase.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region UpdateStatusAsync

    [Fact(DisplayName = "UpdateStatusAsync to Aprovada should update and return entity")]
    public async Task UpdateStatusAsync_ToAprovada_ShouldUpdateAndReturn()
    {
        // Arrange
        var id = Guid.NewGuid();
        var proposta = new PropostaEntity("Cliente", 1500m);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(proposta);

        _repositoryMock
            .Setup(r => r.UpdateAsync(proposta, It.IsAny<CancellationToken>()))
            .ReturnsAsync(proposta);

        // Act
        var result = await _useCase.UpdateStatusAsync(id, PropostaStatus.Aprovada);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PropostaStatus.Aprovada, result.Status);
        _repositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(proposta, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "UpdateStatusAsync to Rejeitada should update and return entity")]
    public async Task UpdateStatusAsync_ToRejeitada_ShouldUpdateAndReturn()
    {
        // Arrange
        var id = Guid.NewGuid();
        var proposta = new PropostaEntity("Cliente", 1500m);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(proposta);

        _repositoryMock
            .Setup(r => r.UpdateAsync(proposta, It.IsAny<CancellationToken>()))
            .ReturnsAsync(proposta);

        // Act
        var result = await _useCase.UpdateStatusAsync(id, PropostaStatus.Rejeitada);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PropostaStatus.Rejeitada, result.Status);
        _repositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(proposta, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "UpdateStatusAsync with non-existing id should return null")]
    public async Task UpdateStatusAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropostaEntity?)null);

        // Act
        var result = await _useCase.UpdateStatusAsync(id, PropostaStatus.Aprovada);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<PropostaEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion
}
