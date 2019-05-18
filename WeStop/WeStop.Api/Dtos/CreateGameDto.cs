using System;
using WeStop.Api.Classes;

namespace WeStop.Api.Dtos
{
    public class CreateGameDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public GameOptions GameOptions { get; set; }
    }
}