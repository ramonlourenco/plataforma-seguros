using Contratacao.Domain.Entities;
using Contratacao.Domain.Ports;

namespace Contratacao.Infrastructure.Repositories;

public class ContratacaoRepository : IContratacaoRepository
{
    private readonly ContratacaoDbContext _context;

    public ContratacaoRepository(ContratacaoDbContext context)
    {
        _context = context;
    }

    public async Task<ContratacaoEntity> AddAsync(ContratacaoEntity contratacao, CancellationToken cancellationToken = default)
    {
        _context.Contratacoes.Add(contratacao);
        await _context.SaveChangesAsync(cancellationToken);
        return contratacao;
    }
}