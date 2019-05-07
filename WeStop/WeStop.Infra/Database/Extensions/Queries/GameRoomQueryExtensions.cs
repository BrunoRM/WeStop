using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Domain;

namespace WeStop.Infra.Extensions.Queries
{
    public static class GameRoomQueryExtensions
    {
        public static async Task<bool> NameExistsAsync(this IQueryable<GameRoom> gameRooms, string name) =>
            await gameRooms.FirstOrDefaultAsync(x => x.Name == name) != null;
    }
}
