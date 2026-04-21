using MediatR;
using Result.Domain.Models;

namespace Lanchonete.Application.Pedido.Dtos.Requests;

public class AtualizarPedidoRequestDto : IRequest<ResultViewModel<bool>>
{
    public int Id { get; set; }
    public AtualizarPedidoRequestAuxDto Dados { get; set; }
}

public class AtualizarPedidoRequestAuxDto
{
    public IEnumerable<int> IdsItensCardapio { get; set; } = [];
}