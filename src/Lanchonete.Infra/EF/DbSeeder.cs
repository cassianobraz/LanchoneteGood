using Lanchonete.Domain.Models.CardapioAggregate;
using Lanchonete.Infra.EF.DbCOntext;
using Microsoft.EntityFrameworkCore;

namespace Lanchonete.Infra.EF;

public static class DbSeeder
{
    public static async Task SeedAsync(LanchoneteContext context)
    {
        if (await context.Set<Cardapio>().AnyAsync())
            return;

        var itens = new List<Cardapio>
        {
            new("X Burger", 5.00m, TipoItemCardapio.Sanduiche),
            new("X Egg", 4.50m, TipoItemCardapio.Sanduiche),
            new("X Bacon", 7.00m, TipoItemCardapio.Sanduiche),

            new("Batata frita", 2.00m, TipoItemCardapio.Acompanhamento),
            new("Refrigerante", 2.50m, TipoItemCardapio.Acompanhamento)
        };

        await context.AddRangeAsync(itens);
        await context.SaveChangesAsync();
    }
}