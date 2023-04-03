﻿using AutoMapper;
using Chatter.Security.DataAccess.Models;

namespace Chatter.Security.Core.Mapping.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IdentityModel, Identity>();
            CreateMap<CreateIdentityModel, CreateIdentity>();
            CreateMap<UpdateIdentityModel, UpdateIdentity>();
        }
    }

}
