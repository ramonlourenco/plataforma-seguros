using Proposta.Domain.Entities;

namespace Proposta.Domain.Ports;

public interface IPropostaRepository
{
    Task<Entities.PropostaEntity> AddAsync(Entities.PropostaEntity proposta, CancellationToken cancellationToken = default);
    Task<Entities.PropostaEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.PropostaEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Entities.PropostaEntity> UpdateAsync(Entities.PropostaEntity proposta, CancellationToken cancellationToken = default);
}
