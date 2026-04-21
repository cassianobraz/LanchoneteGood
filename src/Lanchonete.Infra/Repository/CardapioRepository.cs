using Lanchonete.Domain.Models.CardapioAggregate;
using Lanchonete.Infra.EF.DbCOntext;
using Microsoft.EntityFrameworkCore;

namespace Lanchonete.Infra.Repository;

public class CardapioRepository(LanchoneteContext context) : ICardapioRepository
{
    private readonly LanchoneteContext _context = context;

    public async Task<IEnumerable<Cardapio>> ListarAsync(CancellationToken cancellationToken)
    {
        return await _context.Set<Cardapio>()
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ThenBy(x => x.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Cardapio>> ObterPorIdsAsync(IEnumerable<int> ids, CancellationToken ct)
    {
        return await _context.Cardapio
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(ct);
    }
}