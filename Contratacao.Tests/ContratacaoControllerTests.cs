using System.Threading;
using Contratacao.Api.Controllers;
using Contratacao.Application.UseCases;
using Contratacao.Domain.Entities;
using Contratacao.Domain.Ports;
using Contratacao.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Contratacao.Tests;

public class ContratacaoControllerTests
{
    [Fact(DisplayName = "Contract when use case returns null returns bad request")]
    public async Task Contract_WhenContratacaoUseCaseReturnsNull_ReturnsBadRequest()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var propostaClientMock = new Mock<IPropostaServiceClient>();
        var contratacaoRepositoryMock = new Mock<IContratacaoRepository>();
        propostaClientMock
            .Setup(x => x.GetPropostaByIdAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Proposta?)null);

        var useCase = new ContratarPropostaUseCase(propostaClientMock.Object, contratacaoRepositoryMock.Object);
        var controller = new ContratacaoController(useCase);
        var request = new ContratacaoRequest(propostaId);

        // Act
        var result = await controller.Contract(request);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact(DisplayName = "Contract when use case returns entity returns OK")]
    public async Task Contract_WhenContratacaoUseCaseReturnsEntity_ReturnsOk()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var propostaClientMock = new Mock<IPropostaServiceClient>();
        var contratacaoRepositoryMock = new Mock<IContratacaoRepository>();
        var contratacao = new ContratacaoEntity(propostaId);
        propostaClientMock
            .Setup(x => x.GetPropostaByIdAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Proposta(propostaId, "Cliente", 1500m, StatusProposta.Aprovada));

        contratacaoRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<ContratacaoEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contratacao);

        var useCase = new ContratarPropostaUseCase(propostaClientMock.Object, contratacaoRepositoryMock.Object);
        var controller = new ContratacaoController(useCase);
        var request = new ContratacaoRequest(propostaId);

        // Act
        var result = await controller.Contract(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.NotNull(okResult.Value);
    }
}
