using Microsoft.EntityFrameworkCore;
using WeStop.Domain;
using WeStop.Infra.Maps;

namespace WeStop.Infra
{
    public class WeStopDbContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<GameRoom> GameRooms { get; set; }
        public DbSet<Theme> Themes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server=127.0.0.1;Port=5432;Database=westop;User Id=postgres;Password=admin");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Entity>();

            modelBuilder.ApplyConfiguration(new PlayerMap());
            modelBuilder.ApplyConfiguration(new GameRoomMap());
            modelBuilder.ApplyConfiguration(new ThemeMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}
