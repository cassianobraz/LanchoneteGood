using Lanchonete.Domain.Models.CardapioAggregate;
using Lanchonete.Domain.Models.PedidoAggregate;
using Lanchonete.Infra.EF.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Lanchonete.Infra.EF.DbCOntext;

public class LanchoneteContext : Microsoft.EntityFrameworkCore.DbContext
{
    public LanchoneteContext(DbContextOptions<LanchoneteContext> options) : base(options) { }

    public DbSet<Cardapio> Cardapio { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CardapioConfiguration());
        modelBuilder.ApplyConfiguration(new PedidoConfiguration()); 
    }
}
