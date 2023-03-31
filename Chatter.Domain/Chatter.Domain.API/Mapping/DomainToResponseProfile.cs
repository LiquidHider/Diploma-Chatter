using AutoMapper;
using Chatter.Domain.API.Models.PrivateChat;
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
            CreateMap<PrivateChatMessage, ChatMessageResponse>();

            //Requests
            CreateMap<UpdateChatMessageRequest, UpdateMessage>();
            CreateMap<CreateChatMessageRequest, CreateChatMessage> ();
            CreateMap<OpenPrivateChatRequest, PrivateChat>();
           
        }
    }
}
