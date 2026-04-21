using Result.Domain.Models;

namespace Lanchonete.Domain.Service.Interfaces;

public interface IPedidoService
{
    Task<ResultViewModel<bool>> CriarAsync(IEnumerable<int> idsItensCardapio, CancellationToken ct);
    Task<ResultViewModel<bool>> AtualizarAsync(int idPedido, IEnumerable<int> idsItensCardapio, CancellationToken ct);
    Task<ResultViewModel<bool>> DeletarAsync(int id, CancellationToken ct);
}
