using AutoFixture;
using Chatter.Domain.DataAccess.Models;

namespace Chatter.Domain.Tests.IntegrationTests.DataAccess.FixturesHelpers
{
    internal class ChatUserFixtureHelper
    {
        private Fixture _fixture;
        public ChatUserFixtureHelper()
        {
            _fixture = new Fixture();
        }

        public ChatUserModel CreateRandomChatUser()
        {
            return new ChatUserModel()
            {
                ID = Guid.NewGuid(),
                LastName = _fixture.Create<string>().Substring(0, 20),
                FirstName = _fixture.Create<string>().Substring(0, 20),
                Patronymic = _fixture.Create<string>().Substring(0, 20),
                Email = _fixture.Create<string>().Substring(0, 20),
                UniversityName = _fixture.Create<string>().Substring(0, 20),
                UniversityFaculty = _fixture.Create<string>().Substring(0, 20),
                JoinedUtc = _fixture.Create<DateTime>(),
                LastActiveUtc = _fixture.Create<DateTime>(),
                IsBlocked = _fixture.Create<bool>(),
                BlockedUntilUtc = _fixture.Create<DateTime>()
            };
        }
        public List<ChatUserModel> CreateRandomUsersList(int count)
        {
            var list = new List<ChatUserModel>();
            for (int i = 0; i < count; i++)
            {
                list.Add(CreateRandomChatUser());
            }
            return list;
        }
    }
}
