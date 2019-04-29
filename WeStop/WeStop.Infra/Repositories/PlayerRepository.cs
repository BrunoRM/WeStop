using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WeStop.Domain;
using WeStop.Domain.Repositories;

namespace WeStop.Infra.Repositories
{
    public class PlayerRepository : RepositoryBase, IPlayerRepository
    {
        public PlayerRepository(WeStopDbContext db) : base(db)
        {
        }

        public async Task AddAsync(Player player)
        {
            await _db.Players.AddAsync(player);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> EmailAlreadyExistsAsync(string email) =>
            await _db.Players.FirstOrDefaultAsync(x => x.Email == email) != null;

        public async Task<bool> UserNameAlreadyExistsAsync(string userName) =>
            await _db.Players.FirstOrDefaultAsync(x => x.UserName== userName) != null;
    }
}
