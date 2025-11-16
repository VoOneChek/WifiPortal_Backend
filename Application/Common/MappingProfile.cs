using Application.DTOs.AuthMethodDto;
using Application.DTOs.AuthSessionDto;
using Application.DTOs.UserDto;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.MappingProfile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AuthMethod, ReadAuthMethodDto>();
            CreateMap<AuthSession, ReadAuthSessionDto>();
            CreateMap<User, ReadUserDto>();
            CreateMap<CreateUserDto, User>();
        }
    }
}
