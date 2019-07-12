using AutoMapper;
using WeStop.Api.Classes;
using WeStop.Api.Dtos;

namespace WeStop.Api.Maps
{
    public class PlayerScoreToPlayerScoreDtoProfile : Profile
    {
        public PlayerScoreToPlayerScoreDtoProfile()
        {
            CreateMap<PlayerScore, PlayerScoreDto>()
                .ForMember(dst => dst.LastRoundPontuation, config =>
                {
                    config.MapFrom((src, dst) =>
                    {
                        return src.RoundPontuation;
                    });
                });
        }
    }
}
