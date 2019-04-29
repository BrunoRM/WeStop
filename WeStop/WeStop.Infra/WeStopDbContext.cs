using Microsoft.EntityFrameworkCore;
using WeStop.Domain;
using WeStop.Infra.Maps;

namespace WeStop.Infra
{
    public class WeStopDbContext : DbContext
    {
        public DbSet<Player> Players { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server=127.0.0.1;Port=5432;Database=westop;User Id=postgres;Password=admin");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Entity>();
            modelBuilder.ApplyConfiguration(new PlayerMap());
            base.OnModelCreating(modelBuilder);
        }
    }
}
