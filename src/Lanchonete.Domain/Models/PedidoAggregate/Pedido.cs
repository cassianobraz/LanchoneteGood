using Lanchonete.Domain.Exceptions;
using Lanchonete.Domain.Models.CardapioAggregate;

namespace Lanchonete.Domain.Models.PedidoAggregate;

public class Pedido
{
    public int Id { get; private set; }

    private readonly List<Cardapio> _itens = new();
    public IReadOnlyCollection<Cardapio> Itens => _itens;

    public decimal Subtotal { get; private set; }
    public decimal Desconto { get; private set; }
    public decimal Total { get; private set; }

    protected Pedido() { }

    public Pedido(int id)
    {
        Id = id;
    }

    public void AdicionarItem(Cardapio item)
    {
        if (_itens.Any(i => i.Tipo == item.Tipo))
            throw new ItemDuplicadoPedidoException();

        _itens.Add(item);
    }

    public void AtualizarItens(IEnumerable<Cardapio> itens)
    {
        _itens.Clear();

        foreach (var item in itens)
            AdicionarItem(item);

        CalcularTotais();
    }

    public void CalcularTotais()
    {
        ValidarPedido();

        Subtotal = _itens.Sum(i => i.Valor);

        var temSanduiche = _itens.Any(i => i.Tipo == TipoItemCardapio.Sanduiche);
        var temBatata = _itens.Any(i => i.Tipo == TipoItemCardapio.Batata);
        var temRefrigerante = _itens.Any(i => i.Tipo == TipoItemCardapio.Refrigerante);

        Desconto = 0;

        if (temSanduiche && temBatata && temRefrigerante)
            Desconto = Subtotal * 0.20m;
        else if (temSanduiche && temRefrigerante)
            Desconto = Subtotal * 0.15m;
        else if (temSanduiche && temBatata)
            Desconto = Subtotal * 0.10m;

        Total = Subtotal - Desconto;
    }

    private void ValidarPedido()
    {
        if (!_itens.Any())
            throw new InvalidOperationException("Pedido não pode ser vazio.");

        if (!_itens.Any(i => i.Tipo == TipoItemCardapio.Sanduiche))
            throw new InvalidOperationException("Pedido deve conter um sanduíche.");
    }
}