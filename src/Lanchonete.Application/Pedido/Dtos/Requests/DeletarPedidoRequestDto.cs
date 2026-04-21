using MediatR;
using Result.Domain.Models;

namespace Lanchonete.Application.Pedido.Dtos.Requests;

public class DeletarPedidoRequestDto : IRequest<ResultViewModel<bool>>
{
    public int Id { get; set; }
}
