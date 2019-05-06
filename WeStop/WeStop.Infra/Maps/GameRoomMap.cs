using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeStop.Domain;

namespace WeStop.Infra.Maps
{
    public class GameRoomMap : IEntityTypeConfiguration<GameRoom>
    {
        public void Configure(EntityTypeBuilder<GameRoom> builder)
        {
            builder.ToTable("gameroom", "public");

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
                .HasColumnType("character varying(255)")
                .IsRequired();

            builder.Property(x => x.NumberOfRounds)
                .HasColumnName("number_of_rounds")
                .IsRequired();

            builder.Property(x => x.NumberOfPlayers)
                .HasColumnName("number_of_players")
                .IsRequired();

            builder.Property(x => x.Status)
                .HasColumnName("status")
                .HasColumnType("character varying(15)")
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(x => x.Expiration)
                .HasColumnName("expiraton_date")
                .IsRequired();

            builder.HasMany(x => x.Players);
        }
    }
}
