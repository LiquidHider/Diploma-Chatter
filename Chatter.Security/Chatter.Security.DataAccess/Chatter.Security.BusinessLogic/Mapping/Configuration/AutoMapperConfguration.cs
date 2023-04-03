using AutoMapper;
using Chatter.Security.Core.Mapping.Profiles;

namespace Chatter.Security.Core.Mapping.Configuration
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
