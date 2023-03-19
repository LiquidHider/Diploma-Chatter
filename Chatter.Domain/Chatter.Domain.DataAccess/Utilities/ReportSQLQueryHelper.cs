using Chatter.Domain.DataAccess.Models.Parameters;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            FROM [chatter].[dbo].[Reports]
            {0}";
           

        public const string ListQuery = @"
            SELECT *
            FROM [dbo].[Reports] 
            {0}
            {1} 
            {2}
            OFFSET @PageSize * (@PageNumber - 1) ROWS 
            FETCH NEXT @PageSize ROWS ONLY";

        public const string CountQuery = @"  
                SELECT COUNT(*)
                FROM [dbo].[Reports]
                {0}";

        private const string FilterTemplate = "AND {0} IN ( {1} )";

        private const string SearchTemplate = "ReportedUserID = @Search";
        
        public string BuildFilters(ReportsListParameters listParameters, DynamicParameters dynamicParameters) 
        {
            if (listParameters == null) 
            {
                return string.Empty;
            }
            
            var builder = new StringBuilder();

            if (listParameters.ReportedUsersIDs != null && listParameters.ReportedUsersIDs.Count > 0) 
            {
                builder.AppendLine(string.Format(FilterTemplate, "ReportedUsersIDs", "@ReportedUsersIDs"));
                dynamicParameters.Add("@ReportedUsersIDs", listParameters.ReportedUsersIDs.ToArray());
            }

            if (listParameters.Search != null) 
            {
                builder.AppendLine(SearchTemplate);
                dynamicParameters.Add("Search",$"\"{listParameters.Search}*\"");
            }
            return builder.ToString();
        }
    }
}
