using Microsoft.EntityFrameworkCore;
using WeStop.Infra.Database.SqlServer.Maps;

namespace WeStop.Infra.Database
{
    public class WeStopSqlServerDbContext : WeStopDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=VTCN001\\SQLEXPRESS; Initial Catalog=WeStop; Integrated Security = true;");
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
