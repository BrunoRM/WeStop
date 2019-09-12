using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Classes;
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
                        return src.GetCurrentState().ToString("g");
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
                        return src.Options.Time;
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
                        return src.GetCurrentRoundNumber();
                    });
                });
        }
    }
}
