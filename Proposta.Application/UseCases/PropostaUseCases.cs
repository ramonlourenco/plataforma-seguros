using Proposta.Domain.Entities;
using Proposta.Domain.Enums;
using Proposta.Domain.Ports;

namespace Proposta.Application.UseCases;

public class PropostaUseCases
{
    private readonly IPropostaRepository _repository;

    public PropostaUseCases(IPropostaRepository repository)
    {
        _repository = repository;
    }

    public async Task<PropostaEntity> CreateAsync(string clienteNome, decimal valor, CancellationToken cancellationToken = default)
    {
        var proposta = new PropostaEntity(clienteNome, valor);
        return await _repository.AddAsync(proposta, cancellationToken);
    }

    public Task<PropostaEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => _repository.GetByIdAsync(id, cancellationToken);

    public Task<IEnumerable<PropostaEntity>> GetAllAsync(CancellationToken cancellationToken = default) => _repository.GetAllAsync(cancellationToken);

    public async Task<PropostaEntity?> UpdateStatusAsync(Guid id, PropostaStatus status, CancellationToken cancellationToken = default)
    {
        var proposta = await _repository.GetByIdAsync(id, cancellationToken);
        if (proposta is null)
            return null;

        switch (status)
        {
            case PropostaStatus.EmAnalise:
                proposta.EmAnalise();
                break;
            case PropostaStatus.Aprovada:
                proposta.Aprovar();
                break;
            case PropostaStatus.Rejeitada:
                proposta.Rejeitar();
                break;
            default:
                proposta.Rejeitar();
                break;
        }

        return await _repository.UpdateAsync(proposta, cancellationToken);
    }
}
