using Microsoft.EntityFrameworkCore;
using WeStop.Domain;

namespace WeStop.Infra
{
    public class WeStopDbContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<GameRoom> GameRooms { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<GameRoomPlayer> GameRoomPlayers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {            
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Entity>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
