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
            .OrderBy(x => x.Tipo)
            .ThenBy(x => x.Nome)
            .ToListAsync(cancellationToken);
    }
}