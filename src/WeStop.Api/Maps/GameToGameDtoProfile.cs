using AutoMapper;
using WeStop.Api.Domain;
using WeStop.Api.Dtos;

namespace WeStop.Api.Maps
{
    public class GameToGameDtoProfile : Profile
    {
        public GameToGameDtoProfile()
        {
            CreateMap<Game, GameDto>()
                .ForMember(dst => dst.State, config =>
                {
                    config.MapFrom((src, dst) =>
                    {
                        return src.State.ToString("g");
                    });
                })
                .ForMember(dst => dst.MaxNumberOfPlayers, config =>
                {
                    config.MapFrom((src, dst) =>
                    {
                        return src.Options.NumberOfPlayers;
                    });
                })
                .ForMember(dst => dst.NumberOfRounds, config =>
                {
                    config.MapFrom((src, dst) =>
                    {
                        return src.Options.Rounds;
                    });
                })
                .ForMember(dst => dst.Time, config =>
                {
                    config.MapFrom((src, dst) =>
                    {
                        return src.Options.RoundTime;
                    });
                })
                .ForMember(dst => dst.Themes, config =>
                {
                    config.MapFrom((src, dst) =>
                    {
                        return src.Options.Themes;
                    });
                })
                .ForMember(dst => dst.CurrentRound, config =>
                {
                    config.MapFrom((src, dst) =>
                    {
                        return src.CurrentRound?.Number ?? 1;
                    });
                });
        }
    }
}
