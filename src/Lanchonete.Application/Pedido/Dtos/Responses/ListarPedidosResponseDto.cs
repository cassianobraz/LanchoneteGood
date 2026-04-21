namespace Lanchonete.Application.Pedido.Dtos.Responses;

public class ListarPedidosResponseDto
{
    public List<ObterPedidoResponseDto> Result { get; set; } = new();
}
