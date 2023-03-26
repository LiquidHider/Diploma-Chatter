using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.DbOptions;
using Chatter.Domain.DataAccess.Interfaces;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.DataAccess.Models.Pagination;
using Chatter.Domain.DataAccess.Models.Parameters;
using Chatter.Domain.DataAccess.Utilities;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;


namespace Chatter.Domain.DataAccess.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ReportSQLQueryHelper _queryHelper;
        private readonly DatabaseOptions _dbOptions;
        private readonly ILogger<ReportRepository> _logger;

        public ReportRepository(IOptions<DatabaseOptions> dbOptions, ILogger<ReportRepository> logger)
        {
            _dbOptions = dbOptions?.Value;
            _logger = logger;
            _queryHelper = new ReportSQLQueryHelper();
        }

        public async Task CreateAsync(ReportModel item, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Making SQL query(Create new Report).");
            var parameters = new DynamicParameters();
            parameters.Add("@ID",item.ID);
            parameters.Add("@ReportedUserID", item.ReportedUserID);
            parameters.Add("@Title", item.Title);
            parameters.Add("@Message", item.Message);

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection)) 
            {
                var commandDefinition = new CommandDefinition(ReportSQLQueryHelper.CreateQuery, parameters, cancellationToken: cancellationToken);
                _logger.LogInformation($"Created query: {commandDefinition.CommandText}");
                var createQuery = await db.ExecuteAsync(commandDefinition);
            }
            _logger.LogInformation("SQL query (Create new report) created and executed successfully.");
        }

        public async Task<DeletionStatus> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Making SQL query(Delete Report).");
            var parameters = new DynamicParameters();
            parameters.Add("@ID", id);
            int deletedRows = 0;

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var commandDefinition = new CommandDefinition(ReportSQLQueryHelper.DeleteQuery, parameters, cancellationToken: cancellationToken);
                _logger.LogInformation($"Created query: {commandDefinition.CommandText}");
                deletedRows = await db.ExecuteAsync(commandDefinition);
            }

            if (deletedRows == 0)
            {
                return DeletionStatus.NotExisted;
            }

            _logger.LogInformation("SQL query (Delete report) created and executed successfully.");
            return DeletionStatus.Deleted;
        }

        public async Task<PaginatedResult<ReportModel, ReportSort>> ListAsync(ReportsListParameters listParameters, CancellationToken cancellationToken)
        {
            if (listParameters == null) 
            {
                throw new ArgumentNullException(nameof(listParameters));
            }

            var parameters = new DynamicParameters();
            parameters.Add("@PageSize", listParameters.PageSize);
            parameters.Add("@PageNumber", listParameters.PageNumber);



            var whereQueryPart = listParameters.ReportedUsersIDs != null && listParameters.ReportedUsersIDs.Count > 0 ?
                _queryHelper.Where(listParameters.ReportedUsersIDs.Select(x => $"ReportedUserID = \'{x}\'").ToArray()) 
                : string.Empty;

            var sortOrder = _queryHelper.OrderBy(listParameters.SortOrder.ToString(),
                listParameters.SortBy.ToString());

            var query = string.Format(ReportSQLQueryHelper.ListQuery, whereQueryPart, sortOrder);
            query += $"\n {string.Format(ReportSQLQueryHelper.CountQuery, whereQueryPart)}";
            
            _logger.LogDebug("Dynamic Parameters: {@parameters}", 
                new { DynamicParameters = parameters, Query = query });
            
            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var command = new CommandDefinition(query,parameters,cancellationToken: cancellationToken);
                var queryResult = await db.QueryMultipleAsync(command);

                var entities = await queryResult.ReadAsync<ReportModel>();
                var totalCount = await queryResult.ReadSingleAsync<int>();

                var totalPages = (int)Math.Ceiling(totalCount / (double)listParameters.PageSize);

                var result = new PaginatedResult<ReportModel, ReportSort> 
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    PageNumber = listParameters.PageNumber,
                    PageSize = listParameters.PageSize,
                    SortOrder = listParameters.SortOrder,
                    SortBy = listParameters.SortBy,
                    Entities = entities.ToList()
                };

                return result;
            }
        }

        public async Task<ReportModel> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", id);
            var filterQueryPart = _queryHelper.Where($"ID = @ID");
            var query = string.Format(ReportSQLQueryHelper.GetOneQuery, filterQueryPart);

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection)) 
            {
                var command = new CommandDefinition(query,parameters, cancellationToken: cancellationToken);
                var result = await db.QueryFirstOrDefaultAsync<ReportModel>(command);

                return result;
            }

        }
    }
}
