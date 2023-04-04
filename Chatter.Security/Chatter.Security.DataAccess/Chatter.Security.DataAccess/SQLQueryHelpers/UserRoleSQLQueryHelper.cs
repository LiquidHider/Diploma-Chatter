
namespace Chatter.Security.DataAccess.SQLQueryHelpers
{
    internal class UserRoleSQLQueryHelper
    {
        public const string CreateQuery = @"
        INSERT INTO [dbo].[UserRoles] ([ID], [UserID], [UserRole], [UserRoleName])
        VALUES (@ID, @IdentityID, @UserRole, @UserRoleName)";

        public const string DeleteQuery = @"
        DELETE FROM [dbo].[UserRoles] 
        WHERE [UserID] = @IdentityID 
        AND [UserRole] = @UserRole";

        public const string GetRoleIdQuery = @"
        SELECT [ID]
        FROM [dbo].[UserRoles]
        WHERE [UserID] = @IdentityID AND [UserRole] = @UserRole";

        public const string UserRolesQuery = @"
        SELECT [UserRole]
        FROM [dbo].[UserRoles]
        WHERE [UserID] = @IdentityID";
    }
}
