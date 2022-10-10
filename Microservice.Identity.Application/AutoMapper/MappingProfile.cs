using AutoMapper;
using Microservice.Identity.Application.Dtos;
using Microservice.Identity.Domain.Entities;

namespace Microservice.Identity.Application.AutoMapper
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDto, UserEntity>()
                .ReverseMap();
        }
    }
}
