using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Domain;

namespace WeStop.Infra.Extensions.Queries
{
    public static class PlayerQueryExtensions
    {
        public static async Task<bool> UserNameExistsAsync(this IQueryable<Player> players, string userName) =>
            await players.FirstOrDefaultAsync(x => x.UserName == userName) != null;

        public static async Task<bool> EmailExistsAsync(this IQueryable<Player> players, string email) =>
            await players.FirstOrDefaultAsync(x => x.Email == email) != null;
    }
}
