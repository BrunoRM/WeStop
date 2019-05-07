using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeStop.Domain;

namespace WeStop.Infra.Database.SqlServer.Maps
{
    public class GameRoomMap : IEntityTypeConfiguration<GameRoom>
    {
        public void Configure(EntityTypeBuilder<GameRoom> builder)
        {
            builder.ToTable("gameroom");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(x => x.Password)
                .HasColumnName("password")
                .HasColumnType("varchar(255)")
                .IsRequired();

            builder.Property(x => x.NumberOfRounds)
                .HasColumnName("number_of_rounds")
                .IsRequired();

            builder.Property(x => x.NumberOfPlayers)
                .HasColumnName("number_of_players")
                .IsRequired();

            builder.Property(x => x.Themes)
                .HasColumnName("themes")
                .HasMaxLength(120)
                .IsRequired();

            builder.Property(x => x.AvailableLetters)
                .HasColumnName("available_letters")
                .HasMaxLength(52)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasColumnName("status")
                .HasColumnType("varchar(15)")
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(x => x.Expiration)
                .HasColumnName("expiraton_date")
                .IsRequired();

            builder.HasMany(x => x.Players)
                .WithOne(x => x.GameRoom);

            builder.Metadata.FindNavigation(nameof(GameRoom.Players))
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
