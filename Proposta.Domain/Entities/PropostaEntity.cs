using System.Text.Json.Serialization;
using Proposta.Domain.Enums;

namespace Proposta.Domain.Entities;

public sealed class PropostaEntity
{
    public Guid Id { get; private set; }
    public string ClienteNome { get; private set; } = null!;
    public decimal Valor { get; private set; }

    [JsonIgnore]
    public int StatusId { get; private set; }

    [JsonIgnore]
    public PropostaStatusEntity StatusReference { get; private set; } = null!;

    [JsonPropertyName("Status")]
    public PropostaStatus Status => (PropostaStatus)StatusId;

    public DateTime CriadaEm { get; private set; }

    private PropostaEntity() { }

    public PropostaEntity(string clienteNome, decimal valor)
    {
        Id = Guid.NewGuid();
        ClienteNome = clienteNome;
        Valor = valor;
        StatusId = (int)PropostaStatus.EmAnalise;
        CriadaEm = DateTime.UtcNow;
    }

    internal PropostaEntity(Guid id, string clienteNome, decimal valor, PropostaStatus status, DateTime criadaEm)
    {
        Id = id;
        ClienteNome = clienteNome;
        Valor = valor;
        StatusId = (int)status;
        CriadaEm = criadaEm;
    }

    public void Aprovar()
    {
        StatusId = (int)PropostaStatus.Aprovada;
    }

    public void Rejeitar()
    {
        StatusId = (int)PropostaStatus.Rejeitada;
    }

    public void EmAnalise()
    {
        StatusId = (int)PropostaStatus.EmAnalise;
    }
}
