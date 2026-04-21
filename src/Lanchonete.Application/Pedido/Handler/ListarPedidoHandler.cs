using Lanchonete.Application.Pedido.Dtos.Requests;
using Lanchonete.Application.Pedido.Dtos.Responses;
using Lanchonete.Application.Shared.Helper;
using Lanchonete.Domain.Models.PedidoAggregate;
using Lanchonete.Domain.Shared.interfaces;
using MediatR;
using Result.Domain.Models;

namespace Lanchonete.Application.Pedido.Handler;

public class ListarPedidoHandler(
    IPedidoRepository pedidoRepository,
    IMemoryCacheHelper cacheHelper)
    : IRequestHandler<ListarPedidosRequestDto, ResultViewModel<ListarPedidosResponseDto>>
{
    private readonly IPedidoRepository _pedidoRepository = pedidoRepository;
    private readonly IMemoryCacheHelper _cacheHelper = cacheHelper;

    public async Task<ResultViewModel<ListarPedidosResponseDto>> Handle(ListarPedidosRequestDto request, CancellationToken cancellationToken)
    {
        var response = await _cacheHelper.GetOrCreateAsync(
            CacheKeys.Pedidos,
            async () =>
            {
                var pedidos = await _pedidoRepository.ListarTodosAsync(cancellationToken);

                return new ListarPedidosResponseDto
                {
                    Result = pedidos.Select(pedido => new ObterPedidoResponseDto
                    {
                        Id = pedido.Id,
                        Subtotal = pedido.Subtotal,
                        Desconto = pedido.Desconto,
                        Total = pedido.Total,
                        Itens = pedido.Itens.Select(item => new ItemPedidoResponseDto
                        {
                            Id = item.Id,
                            Nome = item.Nome,
                            Valor = item.Valor,
                            Tipo = item.Tipo.ToString()
                        }).ToList()
                    }).ToList()
                };
            },
            TimeSpan.FromMinutes(5)
        );

        return ResultViewModel<ListarPedidosResponseDto>.Success(response);
    }
}