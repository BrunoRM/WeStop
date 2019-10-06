using System;

namespace WeStop.Api.Dtos
{
    public class PlayerDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsReady { get; set; }
        public bool IsOnline { get; set; }
        public int TotalPontuation { get; set; }
    }
}
