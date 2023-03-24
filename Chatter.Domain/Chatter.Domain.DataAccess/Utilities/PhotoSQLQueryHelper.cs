using Chatter.Domain.DataAccess.Models.Parameters;
using Dapper;
using System.Text;

namespace Chatter.Domain.DataAccess.Utilities
{
    internal class PhotoSQLQueryHelper : SQLQueryHelper
    {
        public const string CreateQuery = @"
            INSERT INTO [dbo].[Photos] ([ID],[Url],[IsMain],[UserID])
            VALUES (@ID, @Url, @IsMain, @UserID)
        "; 
        public const string DeleteQuery = @"
        DELETE FROM [dbo].[Photos]
        WHERE [ID] = @ID";

        public const string GetOneQuery = @"
            SELECT TOP (1) *
            FROM [dbo].[Photos]
            {0}";
        public const string ListQuery = @"
            SELECT *
            FROM [dbo].[Photos] 
            {0}
            {1} 
            OFFSET @PageSize * (@PageNumber - 1) ROWS 
            FETCH NEXT @PageSize ROWS ONLY";

        public const string CountQuery = @"  
                SELECT COUNT(*)
                FROM [dbo].[Photos]
                {0}";
    }
}
