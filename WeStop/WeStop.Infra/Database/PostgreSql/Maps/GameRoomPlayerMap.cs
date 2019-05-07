using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeStop.Domain;

namespace WeStop.Infra.Database.PostgreSql.Maps
{
    public class GameRoomPlayerMap : IEntityTypeConfiguration<GameRoomPlayer>
    {
        public void Configure(EntityTypeBuilder<GameRoomPlayer> builder)
        {
            builder.ToTable("gameroom_player", "public");

            builder.HasKey("GameRoomId", "PlayerId");

            builder.Property(x => x.GameRoomId)
                .HasColumnName("gameroom_id")
                .IsRequired();

            builder.Property(x => x.PlayerId)
                .HasColumnName("player_id")
                .IsRequired();

            builder.Property(x => x.IsAdmin)
                .HasColumnName("is_admin")
                .HasDefaultValue(false)
                .IsRequired();

            builder.HasOne(x => x.GameRoom)
                .WithMany(x => x.Players)
                .HasForeignKey(x => x.GameRoomId);
        }
    }
}
