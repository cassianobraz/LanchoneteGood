using Lanchonete.Application.Pedido.Dtos.Requests;
using Lanchonete.Application.Shared.Helper;
using Lanchonete.Domain.Service.Interfaces;
using Lanchonete.Domain.Shared.interfaces;
using MediatR;
using Result.Domain.Models;

namespace Lanchonete.Application.Pedido.Handler;

public class CriarPedidoHandler(
    IPedidoService pedidoService,
    IMemoryCacheHelper cacheHelper)
    : IRequestHandler<CriarPedidoRequestDto, ResultViewModel<bool>>
{
    private readonly IPedidoService _pedidoService = pedidoService;
    private readonly IMemoryCacheHelper _cacheHelper = cacheHelper;

    public async Task<ResultViewModel<bool>> Handle(CriarPedidoRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _pedidoService.CriarAsync(request.IdsItensCardapio, cancellationToken);

        if (result.IsSuccess)
        {
            _cacheHelper.Remove(CacheKeys.Pedidos);
        }

        return result;
    }
}