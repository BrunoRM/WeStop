using AutoMapper;
using WeStop.Api.Dtos;
using WeStop.Core;

namespace WeStop.Api.Maps
{
    public class PlayerToPlayerDtoProfile : Profile
    {
        public PlayerToPlayerDtoProfile()
        {
            CreateMap<Player, PlayerDto>();
        }
    }
}
