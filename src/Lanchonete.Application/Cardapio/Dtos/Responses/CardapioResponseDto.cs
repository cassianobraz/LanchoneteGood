namespace Lanchonete.Application.Cardapio.Dtos.Responses;

public class CardapioResponseDto
{
    public IEnumerable<ItemCardapioDto> Sanduiches { get; set; } = [];
    public IEnumerable<ItemCardapioDto> Acompanhamentos { get; set; } = [];
}

public class ItemCardapioDto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public decimal Valor { get; set; }
}