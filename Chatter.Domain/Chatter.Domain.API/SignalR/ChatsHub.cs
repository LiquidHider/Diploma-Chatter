using Chatter.Domain.BusinessLogic.Models.Create;
using Microsoft.AspNetCore.SignalR;

namespace Chatter.Domain.API.SignalR
{
    public class ChatsHub : Hub
    {
        public async Task SendChatMessage(CreateChatMessage createRequest, string connectionID) 
        {
            await Clients.Client(connectionID).SendAsync("NewChatMessageRecieved", createRequest);
        }
    }
}
