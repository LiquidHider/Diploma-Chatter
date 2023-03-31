using AutoMapper;
using Chatter.Domain.API.Models.PrivateChat;
using Chatter.Domain.BusinessLogic.Models.Abstract;
using Chatter.Domain.BusinessLogic.Models.Create;
using Chatter.Domain.BusinessLogic.Models.Update;

namespace Chatter.Domain.API.Mapping
{
    public class DomainToResponseProfile : Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<ChatMessage, ChatMessageResponse>();
            CreateMap<UpdateMessage, UpdateChatMessageRequest>();
            CreateMap<CreateChatMessage, CreateChatMessageRequest>();
        }
    }
}
