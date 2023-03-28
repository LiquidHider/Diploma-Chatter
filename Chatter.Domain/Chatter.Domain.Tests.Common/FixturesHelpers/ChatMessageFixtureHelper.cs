using AutoFixture;
using Chatter.Domain.DataAccess.Models;

namespace Chatter.Domain.Tests.Common
{
    public class ChatMessageFixtureHelper
    {
        private readonly Fixture _fixture;

        public ChatMessageFixtureHelper()
        {
            _fixture = new Fixture();
        }

        public ChatMessageModel CreateRandomChatMessage(ChatUserModel sender, ChatUserModel recipient)
        {
            var formattedTime = _fixture.Create<DateTime>().ToString("yyyy-MM-dd HH:mm:ss.ff");
            return new ChatMessageModel()
            {
                ID = Guid.NewGuid(),
                Body = _fixture.Create<string>().Substring(0, 20),
                IsEdited = _fixture.Create<bool>(),
                Sent = DateTime.Parse(formattedTime),
                IsRead = _fixture.Create<bool>(),
                Sender = sender.ID,
                RecipientUser = recipient.ID
            };
        }
        public ChatMessageModel CreateRandomChatMessage(ChatUserModel sender, GroupChatModel recipient)
        {
            var formattedTime = _fixture.Create<DateTime>().ToString("yyyy-MM-dd HH:mm:ss.ff");
            return new ChatMessageModel()
            {
                ID = Guid.NewGuid(),
                Body = _fixture.Create<string>().Substring(0, 20),
                IsEdited = _fixture.Create<bool>(),
                Sent = DateTime.Parse(formattedTime),
                IsRead = _fixture.Create<bool>(),
                Sender = sender.ID,
                RecipientGroup = recipient.ID
            };
        }

        public List<ChatMessageModel> CreateRandomChatMessagesList(int count, ChatUserModel sender, ChatUserModel recipient)
        {
            var list = new List<ChatMessageModel>();
            for (int i = 0; i < count; i++) 
            {
               list.Add(CreateRandomChatMessage(sender, recipient));
            }
            return list;
        }

        public List<ChatMessageModel> CreateRandomChatMessagesList(int count, ChatUserModel sender, GroupChatModel recipient)
        {
            var list = new List<ChatMessageModel>();
            for (int i = 0; i < count; i++)
            {
                list.Add(CreateRandomChatMessage(sender, recipient));
            }
            return list;
        }
    }
}
