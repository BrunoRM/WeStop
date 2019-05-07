using AutoMapper;
using System;
using WeStop.Application.Dtos.GameRoom;
using WeStop.Domain;

namespace WeStop.Application.Maps
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<GameRoom, GameRoomDto>()
                .ForMember(x => x.Themes, config =>
                {
                    config.MapFrom((src, dest) =>
                    {
                        return src.Themes?.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    });
                })
                .ForMember(x => x.AvailableLetters, config =>
                {
                    config.MapFrom((src, dest) =>
                    {
                        return src.AvailableLetters?.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    });
                })
                .ForMember(x => x.IsPrivate, config =>
                {
                    config.MapFrom((src, dest) =>
                    {
                        return string.IsNullOrEmpty(src.Password) ? false : true;
                    });
                })
                .ForMember(x => x.Status, config =>
                {
                    config.MapFrom((src, dest) =>
                    {
                        return src.Status.ToString("g");
                    });
                });
        }
    }
}
