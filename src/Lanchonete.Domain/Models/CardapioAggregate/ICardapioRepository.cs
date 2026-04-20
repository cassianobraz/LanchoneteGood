namespace Lanchonete.Domain.Models.CardapioAggregate;

public interface ICardapioRepository
{
    Task<IEnumerable<Cardapio>> ListarAsync(CancellationToken cancellationToken);
}
