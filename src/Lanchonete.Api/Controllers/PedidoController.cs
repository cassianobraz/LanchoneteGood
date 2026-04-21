using Lanchonete.Application.Pedido.Dtos.Requests;
using Lanchonete.Application.Pedido.Dtos.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Result.Domain.Enum;
using Result.Domain.Models;

namespace Lanchonete.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PedidoController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Retorna a lista de todos os pedidos cadastrados no sistema. Caso não existam pedidos, a resposta será um array vazio [].
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ListarPedidosResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ObterPedidos(CancellationToken ct)
    {
        var result = await _mediator.Send(new ListarPedidosRequestDto(), ct);

        if (result is null)
            return NoContent();

        return Ok(result.Value.Result);
    }

    /// <summary>
    /// Retorna um pedido específico com base no ID informado. Caso o pedido não seja encontrado, a API retorna **404 – Not Found**.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ObterPedidoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<Error>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPedidoPorId(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ObterPedidoRequestDto() { Id = id }, ct);

        if (result.IsFailure)
            return NotFound(result.Errors);

        return Ok(result.Value);
    }

    /// <summary>
    /// Cria um novo pedido validando os itens do cardápio e calculando o valor total.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(List<Error>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(List<Error>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoRequestDto request, CancellationToken ct)
    {
        var result = await _mediator.Send(request, ct);

        if (result.IsFailure)
        {
            return result.ErrorType switch
            {
                TipoErro.Domain => BadRequest(result.Errors),
                TipoErro.NotFound => NotFound(result.Errors),
                _ => BadRequest(result.Errors)
            };
        }

        return NoContent();
    }

    /// <summary>
    /// Atualiza um pedido com base no Id informado, aplicando as regras de negócio necessárias.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="requestAux"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(List<Error>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(List<Error>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarPedido(int id, [FromBody] AtualizarPedidoRequestAuxDto requestAux, CancellationToken ct)
    {
        var Request = new AtualizarPedidoRequestDto
        {
            Id = id,
            Dados = requestAux
        };

        var result = await _mediator.Send(Request, ct);

        if (result.IsFailure)
        {
            return result.ErrorType switch
            {
                TipoErro.Domain => BadRequest(result.Errors),
                TipoErro.NotFound => NotFound(result.Errors),
                _ => BadRequest(result.Errors)
            };
        }

        return NoContent();
    }

    /// <summary>
    /// Remove um pedido com base no Id informado. Caso não seja encontrado, retorna **404 – Not Found**.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(List<Error>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletarPedido(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeletarPedidoRequestDto { Id = id }, ct);

        if (result.IsFailure)
            return NotFound(result.Errors);

        return NoContent();
    }
}
