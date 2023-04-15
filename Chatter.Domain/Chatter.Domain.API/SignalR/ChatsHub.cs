using Chatter.Domain.API.Models.PrivateChat;
using Chatter.Domain.BusinessLogic.Interfaces;
using Chatter.Domain.BusinessLogic.Models.Create;
using Microsoft.AspNetCore.SignalR;

namespace Chatter.Domain.API.SignalR
{
    public class ChatsHub : Hub
    {
        private readonly IPrivateChatService _privateChatService;

        public ChatsHub(IPrivateChatService privateChatService)
        {
            _privateChatService = privateChatService;
        }

        public async Task SendChatMessage(CreateChatMessageRequest createRequest) 
        {
            var createModel = new CreateChatMessage() 
            {
                Body = createRequest.Body,
                SenderID = createRequest.SenderID,
                RecipientID =  createRequest.RecipientID
            };

            var message = await _privateChatService.SendMessageAsync(createModel, default);
            await Clients.User(createRequest.RecipientID.ToString()).SendAsync("NewChatMessageRecieved", message);
        }
    }
}
