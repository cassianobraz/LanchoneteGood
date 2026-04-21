using MediatR;
using Result.Domain.Models;

namespace Lanchonete.Application.Pedido.Dtos.Requests;

public class CriarPedidoRequestDto : IRequest<ResultViewModel<bool>>
{
    public IEnumerable<int> IdsItensCardapio { get; set; } = [];
}
