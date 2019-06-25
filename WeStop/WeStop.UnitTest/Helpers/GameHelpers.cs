using WeStop.Api.Classes;

namespace WeStop.UnitTest.Helpers
{
    public static class GameHelpers
    {
        public static void SetAllPlayersReadyForTheNextRound(this Game game)
        {
            foreach (var player in game.Players)
                player.ChangeStatus(true);
        }
    }
}
