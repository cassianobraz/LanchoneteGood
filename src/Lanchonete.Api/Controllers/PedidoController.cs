using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Lanchonete.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PedidoController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CriarPedido(CancellationToken ct)
    {
        await _mediator.Send(ct);

        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterPedidos(CancellationToken ct)
    {
        var result = await _mediator.Send(ct);

        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterPedido(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(ct);

        return Ok(result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AtualizarPedido(Guid id, CancellationToken ct)
    {
        await _mediator.Send(ct);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeletarPedido(Guid id, CancellationToken ct)
    {
        await _mediator.Send(ct);

        return NoContent();
    }
}
