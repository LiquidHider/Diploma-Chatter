using AutoMapper;
using Chatter.Domain.BusinessLogic.Models;
using Chatter.Domain.BusinessLogic.Models.ChatMessages;
using Chatter.Domain.BusinessLogic.Models.Update;
using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.Models;

namespace Chatter.Domain.BusinessLogic.Mapping.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Report, ReportModel>();
            CreateMap<ReportModel, Report>();
            CreateMap<ChatUser, ChatUserModel>()
            .ForMember(dest => dest.LastActive, opt => opt.MapFrom(src => src.LastActiveUtc))
                .ForMember(dest => dest.BlockedUntil, opt => opt.MapFrom(src => src.BlockedUntilUtc));

            CreateMap<ChatUserModel, ChatUser>()
                .ForMember(dest => dest.LastActiveUtc, opt => opt.MapFrom(src => src.LastActive))
                .ForMember(dest => dest.BlockedUntilUtc, opt => opt.MapFrom(src => src.BlockedUntil));

            CreateMap<UpdateMessage, UpdateChatMessageModel>();
            CreateMap<PrivateChatMessage, ChatMessageModel>()
                .ForMember(dest => dest.RecipientUser, opt => opt.MapFrom(src => src.RecipientID));
            CreateMap<GroupChatMessage, ChatMessageModel>()
              .ForMember(dest => dest.RecipientGroup, opt => opt.MapFrom(src => src.Recipient));

            CreateMap<UpdateChatUser, UpdateChatUserModel>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.UserID));

            CreateMap<UpdateChatUserModel, UpdateChatUser>()
                 .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.ID));

            CreateMap<BusinessLogic.Models.Parameters.ChatUserListParameters, DataAccess.Models.Parameters.ChatUserListParameters>();
        }
    }

}
