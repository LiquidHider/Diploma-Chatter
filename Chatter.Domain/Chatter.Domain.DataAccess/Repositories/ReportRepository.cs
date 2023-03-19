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
    internal class ReportRepository : IReportRepository
    {
        private readonly ReportSQLQueryHelper _queryBuilder;
        private readonly DatabaseOptions _dbOptions;
        private readonly ILogger<ReportRepository> _logger;

        public ReportRepository(IOptions<DatabaseOptions> dbOptions, ILogger<ReportRepository> logger)
        {
            _dbOptions = dbOptions?.Value;
            _logger = logger;
            _queryBuilder = new ReportSQLQueryHelper();
        }

        public async Task CreateAsync(Report item, CancellationToken cancellationToken)
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

        public async Task<DeletionStatus> DeleteAsync(Report item, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Making SQL query(Delete Report).");
            var parameters = new DynamicParameters();
            parameters.Add("@ID", item.ID);
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

        public async Task<PaginatedResult<Report, ReportSort>> ListASync(ReportsListParameters listParameters, CancellationToken cancellationToken)
        {
            if (listParameters == null) 
            {
                throw new ArgumentNullException(nameof(listParameters));
            }

            var parameters = new DynamicParameters();
            parameters.Add("@PageSize", listParameters.PageSize);
            parameters.Add("@PageNumber", listParameters.PageNumber);

            var sortBy = _queryBuilder.Where(listParameters.SortBy.ToString());

            var sortOrder = _queryBuilder.OrderBy(listParameters.SortOrder.ToString(),
                listParameters.SortBy.ToString());

            var filterQueryPart = _queryBuilder.BuildFilters(listParameters, parameters);
            var query = string.Format(ReportSQLQueryHelper.ListQuery, sortBy, filterQueryPart, sortOrder);
            query.Concat($"\n {ReportSQLQueryHelper.CountQuery}");
            
            _logger.LogDebug("Dynamic Parameters: {@parameters}", 
                new { DynamicParameters = parameters, Query = query });
            
            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var command = new CommandDefinition(query,parameters,cancellationToken: cancellationToken);
                var queryResult = await db.QueryMultipleAsync(command);

                var entities = await queryResult.ReadAsync<Report>();
                var totalCount = await queryResult.ReadSingleAsync<int>();

                var totalPages = (int)Math.Ceiling(totalCount / (double)listParameters.PageSize);

                var result = new PaginatedResult<Report, ReportSort> 
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

        public async Task<Report> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("ID", id);
            var filterQueryPart = _queryBuilder.Where($"ID = @ID");
            var query = string.Format(ReportSQLQueryHelper.GetOneQuery, filterQueryPart);

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection)) 
            {
                var command = new CommandDefinition(query,parameters, cancellationToken: cancellationToken);
                var result = await db.QuerySingleAsync<Report>(command);

                return result;
            }

        }
    }
}
