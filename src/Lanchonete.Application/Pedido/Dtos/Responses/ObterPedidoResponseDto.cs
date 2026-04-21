namespace Lanchonete.Application.Pedido.Dtos.Responses;

public class ObterPedidoResponseDto
{
    public int Id { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Desconto { get; set; }
    public decimal Total { get; set; }

    public List<ItemPedidoResponseDto> Itens { get; set; } = new();
}

public class ItemPedidoResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public decimal Valor { get; set; }
    public string Tipo { get; set; } = null!;
}