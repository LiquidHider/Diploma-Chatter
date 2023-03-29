namespace Chatter.Domain.DataAccess.Utilities
{
    internal class ChatUserSQLQueryHelper : SQLQueryHelper
    {
        public const string CreateQuery = @"
            INSERT INTO [dbo].[ChatUsers] (
            [ID],
            [LastName],
            [FirstName],
            [Patronymic],
            [UniversityName],
            [UniversityFaculty],
            [JoinedUtc],
            [LastActive],
            [IsBlocked],
            [BlockedUntil])
            VALUES (@ID, @LastName, @FirstName, @Patronymic, @UniversityName, @UniversityFaculty, 
            @JoinedUtc, @LastActiveUtc, @IsBlocked, @BlockedUntilUtc)";

        public const string DeleteQuery = @"
            DELETE FROM [dbo].[ChatUsers]
            WHERE [ID] = @ID";

        public const string GetOneQuery = @"
            SELECT TOP (1) *
            FROM [dbo].[ChatUsers]
            {0}";

        public const string GetUserContacts = @"SELECT DISTINCT [RecipientUser]  
            FROM [dbo].[Messages]
            WHERE [RecipientUser] != @ID
            UNION
            SELECT DISTINCT [Sender]  
            FROM [dbo].[Messages]
            WHERE [Sender] !=  @ID";

        public const string ListQuery = @"
            SELECT *
            FROM [dbo].[ChatUsers] 
            {0}
            {1}
            OFFSET @PageSize * (@PageNumber - 1) ROWS 
            FETCH NEXT @PageSize ROWS ONLY";

        public const string UpdateQuery = @"
            UPDATE [dbo].[ChatUsers]
            SET {0}
            WHERE ID = @ID";

        public const string CountQuery = @"  
            SELECT COUNT(*)
            FROM [dbo].[ChatUsers]
            {0}";
    }
}
