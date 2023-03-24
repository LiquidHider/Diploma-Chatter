using Chatter.Domain.DataAccess.Models.Parameters;
using Dapper;
using System.Text;

namespace Chatter.Domain.DataAccess.Utilities
{
    internal class ReportSQLQueryHelper : SQLQueryHelper
    {
        public const string CreateQuery = @"
           INSERT INTO [dbo].[Reports] ([ID],[ReportedUserID],[Title],[Message]) 
            VALUES(@ID, @ReportedUserID, @Title, @Message)";

        public const string DeleteQuery = @"
        DELETE FROM [dbo].[Reports]
        WHERE [ID] = @ID";

        public const string GetOneQuery = @"
            SELECT TOP (1) *
            FROM [dbo].[Reports]
            {0}";
           

        public const string ListQuery = @"
            SELECT *
            FROM [dbo].[Reports] 
            {0}
            {1} 
            OFFSET @PageSize * (@PageNumber - 1) ROWS 
            FETCH NEXT @PageSize ROWS ONLY";

        public const string CountQuery = @"  
                SELECT COUNT(*)
                FROM [dbo].[Reports]
                {0}";

        private const string FilterTemplate = "AND {0} IN ( {1} )";

        private const string SearchTemplate = "ReportedUserID = @Search";
        
    }
}
