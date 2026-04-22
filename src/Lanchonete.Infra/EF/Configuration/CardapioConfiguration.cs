using Lanchonete.Domain.Models.CardapioAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lanchonete.Infra.EF.Configuration;

public class CardapioConfiguration : IEntityTypeConfiguration<Cardapio>
{
    public void Configure(EntityTypeBuilder<Cardapio> builder)
    {
        builder.ToTable("Cardapio");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        builder.Property(x => x.Nome)
            .HasColumnName("Nome")
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Valor)
            .HasColumnName("Valor")
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(x => x.Tipo)
            .HasColumnName("Tipo")
            .IsRequired()
            .HasConversion<int>();
    }
}