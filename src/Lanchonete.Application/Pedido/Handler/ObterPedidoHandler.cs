using Lanchonete.Application.Pedido.Dtos.Requests;
using Lanchonete.Application.Pedido.Dtos.Responses;
using Lanchonete.Application.Shared.Helper;
using Lanchonete.Domain.Models.PedidoAggregate;
using Lanchonete.Domain.Shared.interfaces;
using MediatR;
using Result.Domain.Enum;
using Result.Domain.Models;
using static Lanchonete.Domain.Shared.CatalogoDeErros;

namespace Lanchonete.Application.Pedido.Handler;

public class ObterPedidoHandler(IPedidoRepository pedidoRepository, IMemoryCacheHelper cacheHelper)
    : IRequestHandler<ObterPedidoRequestDto, ResultViewModel<ObterPedidoResponseDto>>
{
    private readonly IPedidoRepository _pedidoRepository = pedidoRepository;
    private readonly IMemoryCacheHelper _cacheHelper = cacheHelper;

    public async Task<ResultViewModel<ObterPedidoResponseDto>> Handle(ObterPedidoRequestDto request, CancellationToken cancellationToken)
    {
        var response = await _cacheHelper.GetOrCreateAsync(
            CacheKeys.PedidoPorId(request.Id),
            async () =>
            {
                var pedido = await _pedidoRepository.ObterPorIdAsync(request.Id, cancellationToken);

                if (pedido is null)
                    return null;

                return new ObterPedidoResponseDto
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
                };
            },
            TimeSpan.FromMinutes(5)
        );

        if (response is null)
        {
            return ResultViewModel<ObterPedidoResponseDto>.Failure(
                [new Error(PedidoNaoEncontrado, ObterMensagem(PedidoNaoEncontrado))],
                TipoErro.NotFound);
        }

        return ResultViewModel<ObterPedidoResponseDto>.Success(response);
    }
}