using AutoMapper;
using Chatter.Security.API.Models.Login;
using Chatter.Security.Core.Models;

namespace Chatter.Security.API.Mapping
{
    public class CoreToReponseProfile : Profile
    {
        public CoreToReponseProfile()
        {
            //Requests
            CreateMap<SignInRequest, SignInModel>();
        }
    }
}
