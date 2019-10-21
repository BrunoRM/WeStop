using System;

namespace WeStop.Api.Dtos
{
    public class PlayerScoreDto
    {
        public Guid PlayerId { get; set; }
        public string UserName { get; set; }
        public int LastRoundPontuation { get; set; }
        public int GamePontuation { get; set; }
    }
}
