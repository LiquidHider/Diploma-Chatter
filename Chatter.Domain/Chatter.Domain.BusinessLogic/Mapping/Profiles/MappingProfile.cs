﻿using AutoMapper;
using Chatter.Domain.BusinessLogic.Models;
using Chatter.Domain.BusinessLogic.Models.ChatMessages;
using Chatter.Domain.BusinessLogic.Models.Update;
using Chatter.Domain.DataAccess.Models;

namespace Chatter.Domain.BusinessLogic.Mapping.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Report, ReportModel>();
            CreateMap<ReportModel, Report>();
            CreateMap<ChatUser, ChatUserModel>();

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
        }
    }

}
