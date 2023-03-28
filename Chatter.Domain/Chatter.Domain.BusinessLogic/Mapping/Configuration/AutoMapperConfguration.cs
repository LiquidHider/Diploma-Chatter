using AutoMapper;
using Chatter.Domain.BusinessLogic.Mapping.Profiles;

namespace Chatter.Domain.BusinessLogic.Mapping.Configuration
{
    internal class AutoMapperConfguration
    {
        public MapperConfiguration Configure()
        {
            MappingProfile profile = new();
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(profile);
            });
        }
    }
}
