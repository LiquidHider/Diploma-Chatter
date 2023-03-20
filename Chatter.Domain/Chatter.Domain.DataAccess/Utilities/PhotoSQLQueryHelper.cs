using Chatter.Domain.DataAccess.Models.Parameters;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            {2}
            OFFSET @PageSize * (@PageNumber - 1) ROWS 
            FETCH NEXT @PageSize ROWS ONLY";

        public const string CountQuery = @"  
                SELECT COUNT(*)
                FROM [dbo].[Photos]
                {0}";

        private const string FilterTemplate = "AND {0} IN ( {1} )";

        public string BuildFilters(PhotosListParameters listParameters, DynamicParameters dynamicParameters)
        {
            if (listParameters == null)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();

            if (listParameters.UsersIDs != null && listParameters.UsersIDs.Count > 0)
            {
                builder.AppendLine(string.Format(FilterTemplate, "ReportedUsersIDs", "@ReportedUsersIDs"));
                dynamicParameters.Add("@UsersIDs", listParameters.UsersIDs.ToArray());
            }
            return builder.ToString();
        }
    }
}
