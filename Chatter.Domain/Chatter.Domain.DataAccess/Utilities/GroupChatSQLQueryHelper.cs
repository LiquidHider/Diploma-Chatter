using Chatter.Domain.DataAccess.Models;
using Dapper;

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
            FROM [chatter].[dbo].[GroupChats]
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

    }
}
