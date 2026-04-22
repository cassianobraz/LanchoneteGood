using Lanchonete.Domain.Models.CardapioAggregate;
using Lanchonete.Domain.Models.PedidoAggregate;
using Lanchonete.Domain.Service;
using Lanchonete.Domain.Shared.interfaces;
using Moq;
using Result.Domain.Enum;
using static Lanchonete.Domain.Shared.CatalogoDeErros;

namespace Lanchonete.UnitTests.Domain.Services;

public sealed class PedidoTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepository;
    private readonly Mock<ICardapioRepository> _cardapioRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly PedidoService _sut;

    public PedidoTests()
    {
        _pedidoRepository = new Mock<IPedidoRepository>(MockBehavior.Strict);
        _cardapioRepository = new Mock<ICardapioRepository>(MockBehavior.Strict);
        _unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);

        _sut = new PedidoService(
            _pedidoRepository.Object,
            _cardapioRepository.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task CriarAsync_Lista_Vazia_Deve_Retornar_Failure()
    {
        // Arrange
        var ids = Enumerable.Empty<int>();

        // Act
        var result = await _sut.CriarAsync(ids, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(TipoErro.Domain, result.ErrorType);
        Assert.Contains(result.Errors, e => e.Code == PedidoInvalido);

        _cardapioRepository.VerifyNoOtherCalls();
        _pedidoRepository.VerifyNoOtherCalls();
        _unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CriarAsync_Ids_Duplicados_Deve_Retornar_Failure()
    {
        // Arrange
        var ids = new List<int> { 1, 4, 4 };

        // Act
        var result = await _sut.CriarAsync(ids, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(TipoErro.Domain, result.ErrorType);
        Assert.Contains(result.Errors, e => e.Code == ItemDuplicadoNoPedido);

        _cardapioRepository.VerifyNoOtherCalls();
        _pedidoRepository.VerifyNoOtherCalls();
        _unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CriarAsync_Item_Nao_Encontrado_Deve_Retornar_NotFound()
    {
        // Arrange
        var ids = new List<int> { 1, 4, 5 };

        _cardapioRepository
            .Setup(x => x.ObterPorIdsAsync(ids, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ItemDoCardapio> { CriarSanduiche(1), CriarBatata(4) });

        // Act
        var result = await _sut.CriarAsync(ids, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(TipoErro.NotFound, result.ErrorType);
        Assert.Contains(result.Errors, e => e.Code == ItemCardapioNaoEncontrado);

        _pedidoRepository.VerifyNoOtherCalls();
        _unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CriarAsync_Sem_Sanduiche_Deve_Retornar_PedidoInvalido()
    {
        // Arrange
        var ids = new List<int> { 4, 5 };

        _cardapioRepository
            .Setup(x => x.ObterPorIdsAsync(ids, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ItemDoCardapio>
            {
                CriarBatata(4),
                CriarRefrigerante(5)
            });

        // Act
        var result = await _sut.CriarAsync(ids, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(TipoErro.Domain, result.ErrorType);
        Assert.Contains(result.Errors, e => e.Code == PedidoInvalido);
    }

    [Fact]
    public async Task CriarAsync_Duplicidade_De_Tipo_Deve_Retornar_ItemDuplicado()
    {
        // Arrange
        var ids = new List<int> { 4, 5 };

        _cardapioRepository
            .Setup(x => x.ObterPorIdsAsync(ids, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ItemDoCardapio>
            {
                CriarBatata(4),
                new ItemDoCardapio(5, "Outra Batata", 3m, TipoItemCardapio.Batata)
            });

        // Act
        var result = await _sut.CriarAsync(ids, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(TipoErro.Domain, result.ErrorType);
        Assert.Contains(result.Errors, e => e.Code == ItemDuplicadoNoPedido);
    }

    [Fact]
    public async Task CriarAsync_Valido_Deve_Persistir()
    {
        // Arrange
        Pedido? pedidoPersistido = null;
        var ids = new List<int> { 1, 4, 5 };

        _cardapioRepository
            .Setup(x => x.ObterPorIdsAsync(ids, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ItemDoCardapio>
            {
                CriarSanduiche(1),
                CriarBatata(4),
                CriarRefrigerante(5)
            });

        _pedidoRepository
            .Setup(x => x.CriarAsync(It.IsAny<Pedido>(), It.IsAny<CancellationToken>()))
            .Callback<Pedido, CancellationToken>((p, _) => pedidoPersistido = p)
            .Returns(Task.CompletedTask);

        _unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.CriarAsync(ids, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(pedidoPersistido);
        Assert.Equal(3, pedidoPersistido!.Itens.Count);

        _pedidoRepository.Verify(x => x.CriarAsync(It.IsAny<Pedido>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_Pedido_Nao_Existe_Deve_Retornar_NotFound()
    {
        // Arrange
        _pedidoRepository
            .Setup(x => x.ObterPorIdNTAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pedido?)null);

        // Act
        var result = await _sut.AtualizarAsync(1, new List<int> { 1, 4 }, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(TipoErro.NotFound, result.ErrorType);
        Assert.Contains(result.Errors, e => e.Code == PedidoNaoEncontrado);
    }

    [Fact]
    public async Task AtualizarAsync_Itens_Invalidos_Deve_Retornar_Failure()
    {
        // Arrange
        var pedido = new Pedido(1);

        _pedidoRepository
            .Setup(x => x.ObterPorIdNTAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedido);

        // Act
        var result = await _sut.AtualizarAsync(1, new List<int>(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(TipoErro.Domain, result.ErrorType);
        Assert.Contains(result.Errors, e => e.Code == PedidoInvalido);
    }

    [Fact]
    public async Task AtualizarAsync_Valido_Deve_Atualizar()
    {
        // Arrange
        var pedido = new Pedido(1);
        pedido.AdicionarItem(CriarSanduiche(1));
        pedido.AdicionarItem(CriarBatata(4));
        pedido.CalcularTotais();

        var novosIds = new List<int> { 1, 5 };

        _pedidoRepository
            .Setup(x => x.ObterPorIdNTAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedido);

        _cardapioRepository
            .Setup(x => x.ObterPorIdsAsync(novosIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ItemDoCardapio>
            {
                CriarSanduiche(1),
                CriarRefrigerante(5)
            });

        _pedidoRepository
            .Setup(x => x.AtualizarAsync(pedido, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.AtualizarAsync(1, novosIds, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, pedido.Itens.Count);
    }

    [Fact]
    public async Task DeletarAsync_Pedido_Nao_Existe_Deve_Retornar_NotFound()
    {
        // Arrange
        _pedidoRepository
            .Setup(x => x.ObterPorIdNTAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pedido?)null);

        // Act
        var result = await _sut.DeletarAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(TipoErro.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task DeletarAsync_Valido_Deve_Excluir()
    {
        // Arrange
        var pedido = new Pedido(1);

        _pedidoRepository
            .Setup(x => x.ObterPorIdNTAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pedido);

        _pedidoRepository
            .Setup(x => x.ExcluirAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.DeletarAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        _pedidoRepository.Verify(x => x.ExcluirAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static ItemDoCardapio CriarSanduiche(int id)
        => new(id, "X Burger", 5m, TipoItemCardapio.Sanduiche);

    private static ItemDoCardapio CriarBatata(int id)
        => new(id, "Batata frita", 2m, TipoItemCardapio.Batata);

    private static ItemDoCardapio CriarRefrigerante(int id)
        => new(id, "Refrigerante", 2.5m, TipoItemCardapio.Refrigerante);
}