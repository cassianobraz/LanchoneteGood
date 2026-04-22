namespace Lanchonete.Domain.Models.CardapioAggregate;

public class ItemDoCardapio
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public decimal Valor { get; private set; }
    public TipoItemCardapio Tipo { get; private set; }

    protected ItemDoCardapio() { }

    public ItemDoCardapio(int id, string nome, decimal valor, TipoItemCardapio tipo)
    {
        Id = id;
        Nome = nome;
        Valor = valor;
        Tipo = tipo;
    }
}

public enum TipoItemCardapio
{
    Sanduiche = 1,
    Batata = 2,
    Refrigerante = 3
}