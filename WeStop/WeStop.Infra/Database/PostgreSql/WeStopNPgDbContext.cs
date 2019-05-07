using Microsoft.EntityFrameworkCore;
using WeStop.Infra.Database.PostgreSql.Maps;

namespace WeStop.Infra.Database
{
    public class WeStopNPgDbContext : WeStopDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server=127.0.0.1;Port=5432;Database=westop;User Id=postgres;Password=admin");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new PlayerMap());
            modelBuilder.ApplyConfiguration(new GameRoomMap());
            modelBuilder.ApplyConfiguration(new ThemeMap());
            modelBuilder.ApplyConfiguration(new GameRoomPlayerMap());
        }
    }
}
