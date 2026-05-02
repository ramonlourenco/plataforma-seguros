using System.Net;
using System.Text.Json.Serialization;
using Contratacao.Domain.Entities;
using Contratacao.Domain.Ports;
using Contratacao.Domain.ValueObjects;
using Refit;

namespace Contratacao.Infrastructure.Clients;

public class PropostaServiceClient : IPropostaServiceClient
{
    private readonly IPropostaServiceApi _api;

    public PropostaServiceClient(IPropostaServiceApi api)
    {
        _api = api;
    }

    public async Task<Proposta?> GetPropostaByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _api.GetByIdAsync(id, cancellationToken);
            return response is null ? null : MapToDomain(response);
        }
        catch (ApiException apiEx) when (apiEx.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    private static Proposta MapToDomain(PropostaResponseDto response)
        => new(response.Id, response.ClienteNome, response.Valor, StatusProposta.FromName(response.Status));
}

public interface IPropostaServiceApi
{
    [Get("/api/proposta/{id}")]
    Task<PropostaResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

public sealed class PropostaResponseDto
{
    public Guid Id { get; init; }
    public string ClienteNome { get; init; } = null!;
    public decimal Valor { get; init; }
    public string Status { get; init; } = null!;
}
