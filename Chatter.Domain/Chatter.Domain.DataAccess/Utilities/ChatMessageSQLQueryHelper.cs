using Chatter.Domain.DataAccess.Models;
using Dapper;

namespace Chatter.Domain.DataAccess.Utilities
{
    internal class ChatMessageSQLQueryHelper : SQLQueryHelper
    {
        public const string CreateQuery = @"
            INSERT INTO [dbo].[ChatMessages] ([ID],[Body],[IsEdited],[Sent],[IsRead],[Sender],[RecipientUser],[RecipientGroup])
            VALUES (@ID, @Body, @IsEdited, @Sent, @IsRead, @Sender, @RecipientUser, @RecipientGroup)
        ";

        public const string DeleteQuery = @"
        DELETE FROM [dbo].[ChatMessages]
        WHERE [ID] = @ID";

        public const string GetOneQuery = @"
            SELECT TOP (1) *
            FROM [chatter].[dbo].[ChatMessages]
            {0}";


        public const string ListQuery = @"
            SELECT *
            FROM [dbo].[ChatMessages] 
            {0}
            {1}";

        public const string UpdateQuery = @"
            UPDATE [dbo].[ChatMessages]
            SET {0}
            WHERE ID = @ID";

        public void DefineRecipientToQuery(ChatMessageModel item, DynamicParameters parameters) 
        {
            if (item.RecipientUserId != null) 
            {
                parameters.Add("@RecipientUser", item.RecipientUserId);
                parameters.Add("@RecipientGroup", null);
                return;
            }

            if (item.RecipientGroupId != null) 
            {
                parameters.Add("@RecipientUser", null);
                parameters.Add("@RecipientGroup", item.RecipientGroupId);
                return;
            }

            throw new ArgumentException(nameof(item));
        }
    }
}
