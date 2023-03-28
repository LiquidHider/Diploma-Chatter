using AutoFixture;
using Chatter.Domain.DataAccess.Models;

namespace Chatter.Domain.Tests.Common
{
    public class ReportFixtureHelper
    {
        private Fixture _fixture;

        public ReportFixtureHelper()
        {
            _fixture = new Fixture();
        }

        public ReportModel CreateRandomReport(ChatUserModel reportedUser)
        {
            return new ReportModel()
            {
                ID = Guid.NewGuid(),
                ReportedUserID = reportedUser.ID,
                Title = _fixture.Create<string>().Substring(0, 20),
                Message = _fixture.Create<string>().Substring(0, 20)
            };
        }

        public List<ReportModel> CreateRandomReportsList(List<ChatUserModel> users)
        {
            var list = new List<ReportModel>();
            for (int i = 0; i < users.Count; i++)
            {
                list.Add(CreateRandomReport(users[i]));
            }
            return list;
        }
    }
}
