using Lanchonete.Domain.Exceptions;
using Lanchonete.Domain.Models.CardapioAggregate;
using Lanchonete.Domain.Models.PedidoAggregate;
using Lanchonete.Domain.Service.Interfaces;
using Lanchonete.Domain.Shared.interfaces;
using Result.Domain.Enum;
using Result.Domain.Models;
using static Lanchonete.Domain.Shared.CatalogoDeErros;

namespace Lanchonete.Domain.Service;

public class PedidoService(
    IPedidoRepository pedidoRepository,
    ICardapioRepository cardapioRepository,
    IUnitOfWork unitOfWork) : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository = pedidoRepository;
    private readonly ICardapioRepository _cardapioRepository = cardapioRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ResultViewModel<bool>> CriarAsync(IEnumerable<int> idsItensCardapio, CancellationToken ct)
    {
        var ids = idsItensCardapio?.ToList();

        var validacaoIds = ValidarIdsItens(ids);
        if (validacaoIds.IsFailure)
            return validacaoIds;

        var itensCardapio = await ObterItensCardapioAsync(ids!, ct);
        if (itensCardapio is null)
            return Failure(ItemCardapioNaoEncontrado, TipoErro.NotFound);

        var pedido = new Pedido(0);

        var resultadoAdicionarItens = AplicarItensNoPedido(() => AdicionarItensAoPedido(pedido, itensCardapio));
        if (resultadoAdicionarItens.IsFailure)
            return resultadoAdicionarItens;

        await _pedidoRepository.CriarAsync(pedido, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return ResultViewModel<bool>.Success(true);
    }

    public async Task<ResultViewModel<bool>> AtualizarAsync(int id, IEnumerable<int> idsItensCardapio, CancellationToken ct)
    {
        var pedido = await _pedidoRepository.ObterPorIdNTAsync(id, ct);

        if (pedido is null)
            return Failure(PedidoNaoEncontrado, TipoErro.NotFound);

        var ids = idsItensCardapio?.ToList();

        var validacaoIds = ValidarIdsItens(ids);
        if (validacaoIds.IsFailure)
            return validacaoIds;

        var itensCardapio = await ObterItensCardapioAsync(ids!, ct);
        if (itensCardapio is null)
            return Failure(ItemCardapioNaoEncontrado, TipoErro.NotFound);

        var resultadoAtualizarItens = AplicarItensNoPedido(() => pedido.AtualizarItens(itensCardapio));
        if (resultadoAtualizarItens.IsFailure)
            return resultadoAtualizarItens;

        await _pedidoRepository.AtualizarAsync(pedido, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return ResultViewModel<bool>.Success(true);
    }

    public async Task<ResultViewModel<bool>> DeletarAsync(int id, CancellationToken ct)
    {
        var pedido = await _pedidoRepository.ObterPorIdNTAsync(id, ct);

        if (pedido is null)
            return Failure(PedidoNaoEncontrado, TipoErro.NotFound);

        await _pedidoRepository.ExcluirAsync(id, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return ResultViewModel<bool>.Success(true);
    }

    private static ResultViewModel<bool> ValidarIdsItens(List<int>? ids)
    {
        if (ids is null || ids.Count == 0)
            return Failure(PedidoInvalido, TipoErro.Domain);

        if (ids.Count != ids.Distinct().Count())
            return Failure(ItemDuplicadoNoPedido, TipoErro.Domain);

        return ResultViewModel<bool>.Success(true);
    }

    private async Task<List<ItemDoCardapio>?> ObterItensCardapioAsync(List<int> ids, CancellationToken ct)
    {
        var itensCardapio = (await _cardapioRepository.ObterPorIdsAsync(ids, ct)).ToList();

        if (itensCardapio.Count != ids.Count)
            return null;

        return itensCardapio;
    }

    private static ResultViewModel<bool> AplicarItensNoPedido(Action acao)
    {
        try
        {
            acao();
            return ResultViewModel<bool>.Success(true);
        }
        catch (ItemDuplicadoPedidoException)
        {
            return Failure(ItemDuplicadoNoPedido, TipoErro.Domain);
        }
        catch (InvalidOperationException)
        {
            return Failure(PedidoInvalido, TipoErro.Domain);
        }
    }

    private static void AdicionarItensAoPedido(Pedido pedido, IEnumerable<ItemDoCardapio> itensCardapio)
    {
        foreach (var item in itensCardapio)
            pedido.AdicionarItem(item);

        pedido.CalcularTotais();
    }

    private static ResultViewModel<bool> Failure(string codigoErro, TipoErro tipoErro)
    {
        var erros = new List<Error>
        {
            new(codigoErro, ObterMensagem(codigoErro))
        };

        return ResultViewModel<bool>.Failure(erros, tipoErro);
    }
}