using Contratacao.Application.UseCases;
using Contratacao.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Contratacao.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContratacaoController : ControllerBase
{
    private readonly ContratarPropostaUseCase _useCase;

    public ContratacaoController(ContratarPropostaUseCase useCase)
    {
        _useCase = useCase;
    }

    [HttpPost]
    public async Task<IActionResult> Contract([FromBody] ContratacaoRequest request)
    {
        var result = await _useCase.ExecuteAsync(request.PropostaId);
        return result is null ? BadRequest(new { Message = "Proposta não aprovada ou não encontrada." }) : Ok(result);
    }
}

public record ContratacaoRequest(Guid PropostaId);
