using AutoMapper;
using Chatter.Domain.BusinessLogic.Models;
using Chatter.Domain.DataAccess.Models;

namespace Chatter.Domain.BusinessLogic.Mapping.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<Report, ReportModel>()
                .ForMember(dest => dest.ReportedUserID, opt => opt.MapFrom(src => src.ReportedUser.ID));

            CreateMap<ChatUser, ChatUserModel>();
        }
    }

}
