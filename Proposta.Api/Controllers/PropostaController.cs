using Microsoft.AspNetCore.Mvc;
using Proposta.Application.UseCases;
using Proposta.Domain.Enums;
using Proposta.Domain.Entities;

namespace Proposta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropostaController : ControllerBase
{
    private readonly PropostaUseCases _useCases;

    public PropostaController(PropostaUseCases useCases)
    {
        _useCases = useCases;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePropostaRequest request)
    {
        var proposta = await _useCases.CreateAsync(request.ClienteNome, request.Valor);
        return CreatedAtAction(nameof(GetById), new { id = proposta.Id }, proposta);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var proposta = await _useCases.GetByIdAsync(id);
        return proposta is null ? NotFound() : Ok(proposta);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var propostas = await _useCases.GetAllAsync();
        return Ok(propostas);
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdatePropostaStatusRequest request)
    {
        var proposta = await _useCases.UpdateStatusAsync(id, request.Status);
        return proposta is null ? NotFound() : Ok(proposta);
    }
}

public record CreatePropostaRequest(string ClienteNome, decimal Valor);
public record UpdatePropostaStatusRequest(PropostaStatus Status);
