namespace Chatter.Domain.DataAccess.Utilities
{
    internal class GroupChatSQLQueryHelper : SQLQueryHelper
    {
        public const string CreateQuery = @"
           INSERT INTO [dbo].[GroupChats] ([ID],[Name],[Description]) 
            VALUES(@ID, @Name, @Description)";

        public const string DeleteQuery = @"
        DELETE FROM [dbo].[GroupChats]
        WHERE [ID] = @ID";

        public const string GetOneQuery = @"
            SELECT TOP (1) *
            FROM [dbo].[GroupChats]
            {0}";


        public const string ListQuery = @"
            SELECT *
            FROM [dbo].[GroupChats] 
            {0}
            {1}";

        public const string UpdateQuery = @"
            UPDATE [dbo].[GroupChats] 
            SET {0}
            WHERE ID = @ID";

        public const string CreateBlockedGroupUserQuery = @"
           INSERT INTO [dbo].[BlockedGroupChatUsers] ([ID],[GroupID],[UserID], [BlockedUntil]) 
            VALUES(@ID, @GroupID, @UserID, @BlockedUntil)
        ";
        public const string DeleteBlockedGroupUserQuery = @"
            DELETE FROM [dbo].[BlockedGroupChatUsers]
             WHERE [ID] = @ID";
        
        public const string GetOneBlockedGroupUserQuery = @"
            SELECT TOP (1) *
            FROM [dbo].[BlockedGroupChatUsers]
            {0}";

        public const string ListBlockedGroupUsersQuery = @"
            SELECT *
            FROM [dbo].[BlockedGroupChatUsers] 
            {0}";

        public const string CreateGroupParticipantQuery = @"
           INSERT INTO [dbo].[UserJoinedGroups] ([ID],[GroupID],[UserID], [UserRole]) 
            VALUES(@ID, @GroupID, @UserID, @UserRole)";

        public const string DeleteGroupParticipantQuery = @"
            DELETE FROM [dbo].[UserJoinedGroups]
             WHERE [ID] = @ID";

        public const string GetOneGroupParticipantQuery = @"
            SELECT TOP (1) *
            FROM [dbo].[UserJoinedGroups]
            {0}";

        public const string ListGroupParticipantsQuery = @"
            SELECT *
            FROM [dbo].[UserJoinedGroups] 
            {0}
            {1}";
    }
}
