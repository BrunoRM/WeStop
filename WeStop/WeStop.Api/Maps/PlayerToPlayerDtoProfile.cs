using AutoMapper;
using WeStop.Api.Domain;
using WeStop.Api.Dtos;

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
