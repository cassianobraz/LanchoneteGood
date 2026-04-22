using Lanchonete.Domain.Models.CardapioAggregate;
using Lanchonete.Infra.EF.DbCOntext;
using Microsoft.EntityFrameworkCore;

namespace Lanchonete.Infra.Repository;

public class CardapioRepository(LanchoneteContext context) : ICardapioRepository
{
    private readonly LanchoneteContext _context = context;

    public async Task<IEnumerable<ItemDoCardapio>> ListarAsync(CancellationToken cancellationToken)
    {
        return await _context.Set<ItemDoCardapio>()
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ThenBy(x => x.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ItemDoCardapio>> ObterPorIdsAsync(IEnumerable<int> ids, CancellationToken ct)
    {
        return await _context.ItensDoCardapio
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(ct);
    }
}