namespace Chatter.Security.Tests.IntegrationTests.Helper
{
    internal class DataHelper
    {
        public static readonly List<string> TableNameMap = new List<string>
        {
            "dbo.ChatUsers",
            "dbo.Reports",
            "dbo.GroupChats",
            "dbo.Photos",
            "dbo.Messages",
            "dbo.Identities",
            "dbo.Messages",
            "dbo.UserJoinedGroups",
            "dbo.BlockedGroupChatUsers",
        };
    }
}
