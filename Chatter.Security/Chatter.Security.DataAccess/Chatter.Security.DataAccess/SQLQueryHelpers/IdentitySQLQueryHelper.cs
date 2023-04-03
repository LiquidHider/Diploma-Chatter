namespace Chatter.Security.DataAccess.SQLQueryHelpers
{
    internal class IdentitySQLQueryHelper : SQLQueryHelper
    {
        public const string CreateQuery = @"
        INSERT INTO [dbo].[Identities] ([ID], [Email], [UserTag], [PasswordHash], [PasswordKey], [UserID])
        VALUES (@ID, @Email, @UserTag, @PasswordHash, @PasswordKey, @UserID)";

        public const string DeleteQuery = @"DELETE FROM [dbo].[Identities] WHERE [ID] = @ID";

        public const string GetOneQuery = @"
        SELECT TOP (1) *
        FROM [dbo].[Identities]
        WHERE [ID] = @ID";

        public const string GetOneByEmailOrUserTagQuery = @"
        SELECT TOP (1) *
        FROM [dbo].[Identities]
        WHERE [UserTag] = @UserTag
        OR [Email] = @Email";

        public const string UpdateQuery = @"
        UPDATE [dbo].[Identities]
        SET {0}
        WHERE [ID] = @ID";
    }
}
