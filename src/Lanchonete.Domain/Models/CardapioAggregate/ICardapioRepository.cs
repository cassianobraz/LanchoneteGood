namespace Lanchonete.Domain.Models.CardapioAggregate;

public interface ICardapioRepository
{
    Task<IEnumerable<ItemDoCardapio>> ListarAsync(CancellationToken cancellationToken);
    Task<IEnumerable<ItemDoCardapio>> ObterPorIdsAsync(IEnumerable<int> ids, CancellationToken ct);
}
