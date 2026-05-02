using Contratacao.Domain.Entities;

namespace Contratacao.Domain.Ports;

public interface IContratacaoRepository
{
    Task<ContratacaoEntity> AddAsync(ContratacaoEntity contratacao, CancellationToken cancellationToken = default);
}