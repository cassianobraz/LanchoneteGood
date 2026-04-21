namespace Lanchonete.Application.Shared.Helper;

public static class CacheKeys
{
    public const string Cardapio = "cardapio-completo";
    public const string Pedidos = "pedidos-listar-todos";
    public static string PedidoPorId(int id) => $"pedido:{id}";
}