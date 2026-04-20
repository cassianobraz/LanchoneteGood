using Lanchonete.Application.Cardapio.Dtos.Responses;
using MediatR;
using Result.Domain.Models;

namespace Lanchonete.Application.Cardapio.Dtos.Requests;

public class CardapioRequestDto : IRequest<ResultViewModel<CardapioResponseDto>>
{
}
