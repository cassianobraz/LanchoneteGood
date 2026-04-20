using Lanchonete.Application.Cardapio.Dtos.Requests;
using Lanchonete.Application.Cardapio.Dtos.Responses;
using Lanchonete.Domain.Models.CardapioAggregate;
using MediatR;
using Result.Domain.Models;
using Lanchonete.Application.Shared.Helper;

namespace Lanchonete.Application.Cardapio.Handlers;

public class CardapioHandler(ICardapioRepository cardapioRepository, MemoryCacheHelper cacheHelper)
    : IRequestHandler<CardapioRequestDto, ResultViewModel<CardapioResponseDto>>
{
    private const string CardapioCacheKey = "cardapio-completo";

    private readonly ICardapioRepository _cardapioRepository = cardapioRepository;
    private readonly MemoryCacheHelper _cacheHelper = cacheHelper;

    public async Task<ResultViewModel<CardapioResponseDto>> Handle(CardapioRequestDto request, CancellationToken cancellationToken)
    {
        var response = await _cacheHelper.GetOrCreateAsync(
            CardapioCacheKey,
            async () =>
            {
                var itensCardapio = await _cardapioRepository.ListarAsync(cancellationToken);

                return new CardapioResponseDto
                {
                    Sanduiches = itensCardapio
                        .Where(x => x.Tipo == TipoItemCardapio.Sanduiche)
                        .Select(x => new ItemCardapioDto
                        {
                            Nome = x.Nome,
                            Valor = x.Valor
                        })
                        .ToList(),

                    Acompanhamentos = itensCardapio
                        .Where(x => x.Tipo == TipoItemCardapio.Acompanhamento)
                        .Select(x => new ItemCardapioDto
                        {
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