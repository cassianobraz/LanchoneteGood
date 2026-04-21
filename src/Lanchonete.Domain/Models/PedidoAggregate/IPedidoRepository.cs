namespace Lanchonete.Domain.Models.PedidoAggregate;

public interface IPedidoRepository
{
    Task<Pedido?> ObterPorIdAsync(int id, CancellationToken ct);
    Task<IEnumerable<Pedido>> ListarTodosAsync(CancellationToken ct);
    Task CriarAsync(Pedido pedido, CancellationToken ct);
    Task AtualizarAsync(Pedido pedido, CancellationToken ct);
    Task ExcluirAsync(int id, CancellationToken ct);
}
