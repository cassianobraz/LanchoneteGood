using Lanchonete.Application.Cardapio.Dtos.Requests;
using Lanchonete.Application.Cardapio.Dtos.Responses;
using Lanchonete.Application.Shared.Helper;
using Lanchonete.Domain.Models.CardapioAggregate;
using Lanchonete.Domain.Shared.interfaces;
using MediatR;
using Result.Domain.Models;

namespace Lanchonete.Application.Cardapio.Handlers;

public class CardapioHandler(
    ICardapioRepository cardapioRepository,
    IMemoryCacheHelper cacheHelper)
    : IRequestHandler<CardapioRequestDto, ResultViewModel<CardapioResponseDto>>
{
    private readonly ICardapioRepository _cardapioRepository = cardapioRepository;
    private readonly IMemoryCacheHelper _cacheHelper = cacheHelper;

    public async Task<ResultViewModel<CardapioResponseDto>> Handle(CardapioRequestDto request, CancellationToken cancellationToken)
    {
        var response = await _cacheHelper.GetOrCreateAsync(
            CacheKeys.Cardapio,
            async () =>
            {
                var itensCardapio = await _cardapioRepository.ListarAsync(cancellationToken);

                return new CardapioResponseDto
                {
                    Sanduiches = itensCardapio
                        .Where(x => x.Tipo == TipoItemCardapio.Sanduiche)
                        .Select(x => new ItemCardapioDto
                        {
                            Id = x.Id,
                            Nome = x.Nome,
                            Valor = x.Valor
                        })
                        .ToList(),

                    Acompanhamentos = itensCardapio
                        .Where(x =>
                            x.Tipo == TipoItemCardapio.Batata ||
                            x.Tipo == TipoItemCardapio.Refrigerante)
                        .Select(x => new ItemCardapioDto
                        {
                            Id = x.Id,
                            Nome = x.Nome,
                            Valor = x.Valor
                        })
                        .ToList()
                };
            },
            TimeSpan.FromMinutes(30)
        );

        return ResultViewModel<CardapioResponseDto>.Success(response);
    }
}