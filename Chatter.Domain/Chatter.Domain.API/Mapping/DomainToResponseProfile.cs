using AutoMapper;
using Chatter.Domain.API.Models.ChatUser;
using Chatter.Domain.API.Models.PrivateChat;
using Chatter.Domain.API.Models.Reports;
using Chatter.Domain.BusinessLogic.Models;
using Chatter.Domain.BusinessLogic.Models.Abstract;
using Chatter.Domain.BusinessLogic.Models.ChatMessages;
using Chatter.Domain.BusinessLogic.Models.Chats;
using Chatter.Domain.BusinessLogic.Models.Create;
using Chatter.Domain.BusinessLogic.Models.Update;

namespace Chatter.Domain.API.Mapping
{
    public class DomainToResponseProfile : Profile
    {
        public DomainToResponseProfile()
        {
            //Responses
            CreateMap<ChatMessage, ChatMessageResponse>();
            CreateMap<PrivateChatMessage, ChatMessageResponse>().ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.Sender));
            CreateMap<PrivateChat, PrivateChatReponse>();
            CreateMap<Report, ReportResponse>();
            CreateMap<ChatUser, ChatUserResponse>()
                .ForMember(dest => dest.Contacts, opt => opt.MapFrom(src => src.Contacts));
            
            //Requests
            CreateMap<UpdateChatMessageRequest, UpdateMessage>();
            CreateMap<CreateChatMessageRequest, CreateChatMessage>();
            CreateMap<OpenPrivateChatRequest, PrivateChat>();
            CreateMap<CreateChatUserRequest, CreateChatUser>();
            CreateMap<UpdateChatUserRequest, UpdateChatUser>();
            CreateMap<BlockUserRequest, BlockUser>();
            CreateMap<CreateReportRequest, CreateReport>();

        }
    }
}
