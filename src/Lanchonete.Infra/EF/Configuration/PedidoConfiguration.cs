using Lanchonete.Domain.Models.CardapioAggregate;
using Lanchonete.Domain.Models.PedidoAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lanchonete.Infra.EF.Configuration;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedidos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Subtotal)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(p => p.Desconto)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(p => p.Total)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Metadata
            .FindNavigation(nameof(Pedido.Itens))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder
            .HasMany(p => p.Itens)
            .WithMany() 
            .UsingEntity<Dictionary<string, object>>(
                "PedidoItens",
                j => j
                    .HasOne<Cardapio>()
                    .WithMany()
                    .HasForeignKey("CardapioId")
                    .OnDelete(DeleteBehavior.Restrict),

                j => j
                    .HasOne<Pedido>()
                    .WithMany()
                    .HasForeignKey("PedidoId")
                    .OnDelete(DeleteBehavior.Cascade),

                j =>
                {
                    j.HasKey("PedidoId", "CardapioId");

                    j.ToTable("PedidoItens");
                });
    }
}