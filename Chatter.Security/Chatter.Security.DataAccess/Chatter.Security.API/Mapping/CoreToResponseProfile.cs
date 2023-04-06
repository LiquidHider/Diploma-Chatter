using AutoMapper;
using Chatter.Security.API.Models.Login;
using Chatter.Security.Core.Models;

namespace Chatter.Security.API.Mapping
{
    public class CoreToResponseProfile : Profile
    {
        public CoreToResponseProfile()
        {
            //Requests
            CreateMap<SignInRequest, SignInModel>();
        }
    }
}
