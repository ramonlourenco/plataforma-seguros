using Contratacao.Domain.ValueObjects;

namespace Contratacao.Domain.Entities;

public sealed class ContratacaoEntity
{
    public Guid Id { get; private set; }
    public Guid PropostaId { get; private set; }
    public DateTime DataContratacao { get; private set; }
    public StatusProposta Status { get; private set; } = null!;

    private ContratacaoEntity() { }

    public ContratacaoEntity(Guid propostaId)
    {
        Id = Guid.NewGuid();
        PropostaId = propostaId;
        DataContratacao = DateTime.UtcNow;
        Status = StatusProposta.Contratada;
    }
}
