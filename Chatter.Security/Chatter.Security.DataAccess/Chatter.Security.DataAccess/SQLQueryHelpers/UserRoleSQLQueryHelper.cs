
namespace Chatter.Security.DataAccess.SQLQueryHelpers
{
    internal class UserRoleSQLQueryHelper
    {
        public const string CreateQuery = @"
        INSERT INTO [dbo].[Roles] ([ID], [UserID], [UserRole], [UserRoleName])
        VALUES (@ID, @UserID, @UserRole, @UserRoleName)";

        public const string DeleteQuery = @"
        DELETE FROM [dbo].[Roles] 
        WHERE [UserID] = @UserID 
        AND [UserRole] = @UserRole";

        public const string GetRoleQuery = @"
        SELECT *
        FROM [dbo].[Identities]
        WHERE [UserID] = @UserID AND [UserRole] = @UserRole";

        public const string UserRolesQuery = @"
        SELECT *
        FROM [dbo].[Identities]
        WHERE [UserID] = @UserID";
    }
}
