namespace Lanchonete.Domain.Shared;

public class CatalogoDeErros
{
    public static string PedidoNaoEncontrado => "LCG0001";
    public static string PedidoInvalido => "LCG0002";
    public static string ItemCardapioNaoEncontrado => "LCG0003";
    public static string ItemDuplicadoNoPedido => "LCG0004";

    public static string ObterMensagem(string codigoErro) => Resources.ResourceManager.GetString(codigoErro) ?? "Erro Interno";
}

