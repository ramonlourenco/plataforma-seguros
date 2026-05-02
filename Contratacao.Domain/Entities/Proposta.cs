using Contratacao.Domain.ValueObjects;

namespace Contratacao.Domain.Entities;

public sealed class Proposta
{
    public Guid Id { get; init; }
    public string ClienteNome { get; init; } = null!;
    public decimal Valor { get; init; }
    public StatusProposta Status { get; init; } = null!;

    public Proposta(Guid id, string clienteNome, decimal valor, StatusProposta status)
    {
        Id = id;
        ClienteNome = clienteNome;
        Valor = valor;
        Status = status;
    }
}
