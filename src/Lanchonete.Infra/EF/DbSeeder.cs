using Lanchonete.Domain.Models.CardapioAggregate;
using Lanchonete.Infra.EF.DbCOntext;
using Microsoft.EntityFrameworkCore;

namespace Lanchonete.Infra.EF;

public static class DbSeeder
{
    public static async Task SeedAsync(LanchoneteContext context)
    {
        var itens = new List<ItemDoCardapio>
        {
            new(1, "X Burger", 5.00m, TipoItemCardapio.Sanduiche),
            new(2, "X Egg", 4.50m, TipoItemCardapio.Sanduiche),
            new(3, "X Bacon", 7.00m, TipoItemCardapio.Sanduiche),
            new(4, "Batata frita", 2.00m, TipoItemCardapio.Batata),
            new(5, "Refrigerante", 2.50m, TipoItemCardapio.Refrigerante)
        };

        foreach (var item in itens)
        {
            var existente = await context.Set<ItemDoCardapio>()
                .FirstOrDefaultAsync(x => x.Id == item.Id);

            if (existente is null)
            {
                await context.Set<ItemDoCardapio>().AddAsync(item);
            }
            else
            {
                context.Entry(existente).CurrentValues.SetValues(item);
            }
        }

        await context.SaveChangesAsync();
    }
}