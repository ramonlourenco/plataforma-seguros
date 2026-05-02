using Contratacao.Domain.Entities;

namespace Contratacao.Domain.Ports;

public interface IPropostaServiceClient
{
    Task<Proposta?> GetPropostaByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
