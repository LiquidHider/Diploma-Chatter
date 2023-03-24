using AutoFixture;
using Chatter.Domain.DataAccess.Models;

namespace Chatter.Domain.Tests.IntegrationTests.DataAccess.FixturesHelpers
{
    internal class PhotoFixtureHelper
    {
        private readonly Fixture _fixture;
        public PhotoFixtureHelper() 
        {
            _fixture = new Fixture();

        }

        public PhotoModel CreateRandomPhoto(ChatUserModel photoOwner)
        {
            return new PhotoModel()
            {
                ID = Guid.NewGuid(),
                Url = _fixture.Create<string>(),
                IsMain = _fixture.Create<bool>(),
                UserID = photoOwner.ID,
            };
        }

        public List<PhotoModel> CreateRandomReportsList(List<ChatUserModel> users)
        {
            var list = new List<PhotoModel>();
            for (int i = 0; i < users.Count; i++)
            {
                list.Add(CreateRandomPhoto(users[i]));
            }
            return list;
        }
    }
}
