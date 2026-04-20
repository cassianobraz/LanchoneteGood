namespace Lanchonete.Domain.Models.CardapioAggregate;

public class Cardapio
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public decimal Valor { get; private set; }
    public TipoItemCardapio Tipo { get; private set; }

    protected Cardapio() { }

    public Cardapio(string nome, decimal valor, TipoItemCardapio tipo)
    {
        Nome = nome;
        Valor = valor;
        Tipo = tipo;
    }
}

public enum TipoItemCardapio
{
    Sanduiche = 1,
    Acompanhamento = 2
}