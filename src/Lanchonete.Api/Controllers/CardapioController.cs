using Lanchonete.Application.Cardapio.Dtos.Requests;
using Lanchonete.Application.Cardapio.Dtos.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Lanchonete.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CardapioController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(CardapioResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterCardapio(CancellationToken ct)
    {
        var result = await _mediator.Send(new CardapioRequestDto(), ct);

        return Ok(result.Value);
    }
}
