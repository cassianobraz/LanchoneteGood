namespace Lanchonete.Domain.Models.CardapioAggregate;

public interface ICardapioRepository
{
    Task<IEnumerable<Cardapio>> ListarAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Cardapio>> ObterPorIdsAsync(IEnumerable<int> ids, CancellationToken ct);
}
