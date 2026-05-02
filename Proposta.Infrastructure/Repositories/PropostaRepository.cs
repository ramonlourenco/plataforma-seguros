using Microsoft.EntityFrameworkCore;
using Proposta.Domain.Entities;
using Proposta.Domain.Ports;

namespace Proposta.Infrastructure.Repositories;

public class PropostaRepository : IPropostaRepository
{
    private readonly PropostaDbContext _context;

    public PropostaRepository(PropostaDbContext context)
    {
        _context = context;
    }

    public async Task<PropostaEntity> AddAsync(PropostaEntity proposta, CancellationToken cancellationToken = default)
    {
        _context.Propostas.Add(proposta);
        await _context.SaveChangesAsync(cancellationToken);
        return proposta;
    }

    public async Task<IEnumerable<PropostaEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Propostas.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<PropostaEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Propostas.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<PropostaEntity> UpdateAsync(PropostaEntity proposta, CancellationToken cancellationToken = default)
    {
        _context.Propostas.Update(proposta);
        await _context.SaveChangesAsync(cancellationToken);
        return proposta;
    }
}
