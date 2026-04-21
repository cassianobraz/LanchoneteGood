using Lanchonete.Application.Pedido.Dtos.Responses;
using MediatR;
using Result.Domain.Models;

namespace Lanchonete.Application.Pedido.Dtos.Requests;

public class ObterPedidoRequestDto : IRequest<ResultViewModel<ObterPedidoResponseDto>>
{
    public int Id { get; set; }
}
