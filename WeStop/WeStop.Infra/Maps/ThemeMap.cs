using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeStop.Domain;

namespace WeStop.Infra.Maps
{
    public class ThemeMap : IEntityTypeConfiguration<Theme>
    {
        public void Configure(EntityTypeBuilder<Theme> builder)
        {
            builder.ToTable("theme", "public");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .IsRequired();

            builder.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(20)
                .IsRequired();

            builder.HasData(
                new Theme("Nome"),
                new Theme("Sobrenome"),
                new Theme("CEP"),
                new Theme("FDS"),
                new Theme("Carro"),
                new Theme("Marca"),
                new Theme("Objeto"),
                new Theme("Cor"),
                new Theme("Fruta")
            );
        }
    }
}
