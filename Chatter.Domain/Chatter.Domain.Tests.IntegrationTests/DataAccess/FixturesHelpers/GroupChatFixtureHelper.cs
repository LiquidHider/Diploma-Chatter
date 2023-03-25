using AutoFixture;
using Chatter.Domain.DataAccess.Models;

namespace Chatter.Domain.Tests.IntegrationTests.DataAccess.FixturesHelpers
{
    internal class GroupChatFixtureHelper
    {
        private readonly Fixture _fixture;

        public GroupChatFixtureHelper()
        {
            _fixture = new Fixture();
        }

        public GroupChatModel CreateRandomGroupChat() 
        {
            return new GroupChatModel() 
            {
                ID = Guid.NewGuid(),
                Name = _fixture.Create<string>().Substring(0,20),
                Description = _fixture.Create<string>().Substring(0, 20),
            };
        }

        public List<GroupChatModel> CreateRandomGroupChatsList(int count)
        {
            var list = new List<GroupChatModel>();
            for (int i = 0; i < count; i++)
            {
                list.Add(CreateRandomGroupChat());
            }
            return list;
        }
    }
}
