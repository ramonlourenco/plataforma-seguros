using System.Net.Http;
using Contratacao.Domain.Entities;
using Contratacao.Domain.Ports;
using Contratacao.Domain.ValueObjects;

namespace Contratacao.Application.UseCases;

public class ContratarPropostaUseCase
{
    private readonly IPropostaServiceClient _propostaClient;
    private readonly IContratacaoRepository _contratacaoRepository;

    public ContratarPropostaUseCase(IPropostaServiceClient propostaClient, IContratacaoRepository contratacaoRepository)
    {
        _propostaClient = propostaClient;
        _contratacaoRepository = contratacaoRepository;
    }

    public async Task<ContratacaoEntity?> ExecuteAsync(Guid propostaId, CancellationToken cancellationToken = default)
    {
        try
        {
            var proposta = await _propostaClient.GetPropostaByIdAsync(propostaId, cancellationToken);
            if (proposta is null || proposta.Status != StatusProposta.Aprovada)
                return null;

            var contratacao = new ContratacaoEntity(propostaId);
            return await _contratacaoRepository.AddAsync(contratacao, cancellationToken);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }
}
