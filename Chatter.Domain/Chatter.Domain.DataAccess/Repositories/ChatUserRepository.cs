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
using System.Text;

namespace Chatter.Domain.DataAccess.Repositories
{
    public class ChatUserRepository : IChatUserRepository
    {
        private readonly DatabaseOptions _dbOptions;
        private readonly ChatUserSQLQueryHelper _queryHelper;
        private readonly ILogger<ChatUserRepository> _logger;

        public ChatUserRepository(IOptions<DatabaseOptions> dbOptions, ILogger<ChatUserRepository> logger)
        {
            _queryHelper = new ChatUserSQLQueryHelper();
            _logger = logger;
            _dbOptions = dbOptions?.Value;
        }

        public async Task CreateAsync(ChatUserModel item, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", item.ID);
            parameters.Add("@LastName", item.LastName);
            parameters.Add("@FirstName", item.FirstName);
            parameters.Add("@Patronymic", item.Patronymic);
            parameters.Add("@UniversityName", item.UniversityName);
            parameters.Add("@UniversityFaculty", item.UniversityFaculty);
            parameters.Add("@JoinedUtc", item.JoinedUtc);
            parameters.Add("@LastActiveUtc", item.LastActive);
            parameters.Add("@IsBlocked", item.IsBlocked);
            parameters.Add("@BlockedUntilUtc", item.BlockedUntil);

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var commandDefinition = new CommandDefinition(ChatUserSQLQueryHelper.CreateQuery, parameters, cancellationToken: cancellationToken);
                _logger.LogInformation($"Created query: {commandDefinition.CommandText}");
                var createQuery = await db.ExecuteAsync(commandDefinition);
            }
        }

        public async Task<DeletionStatus> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", id);
            int deletedRows = 0;

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var command = new CommandDefinition(ChatUserSQLQueryHelper.DeleteQuery, parameters, cancellationToken: cancellationToken);
                deletedRows = await db.ExecuteAsync(command);
            }

            if (deletedRows == 0)
            {
                return DeletionStatus.NotExisted;
            }

            return DeletionStatus.Deleted;
        }

        public async Task<PaginatedResult<ChatUserModel,ChatUserSort>> ListAsync(ChatUserListParameters listParameters, CancellationToken cancellationToken)
        {
            if (listParameters == null)
            {
                throw new ArgumentException(nameof(listParameters));
            }
            var parameters = new DynamicParameters();
            parameters.Add("@PageSize", listParameters.PageSize);
            parameters.Add("@PageNumber", listParameters.PageNumber);

            var filtersList = new List<string>();
            if (listParameters.UniversitiesNames != null) 
            {
                filtersList = filtersList.Concat(listParameters.UniversitiesNames.Select(x => $"[UniversityName] = '{x}'")).ToList();
            }
            if (listParameters.UniversitiesFaculties != null)
            {
                filtersList = filtersList.Concat(listParameters.UniversitiesFaculties.Select(x => $"[UniversityFaculty] = '{x}'")).ToList();
            }

            var filter = filtersList.Count > 0 ? _queryHelper.Where(filtersList.ToArray()) : string.Empty;

            var sortOrder = _queryHelper.OrderBy(listParameters.SortOrder.ToString(), listParameters.SortBy.ToString());

            var query = string.Format(ChatUserSQLQueryHelper.ListQuery, filter, sortOrder);
            query += $"\n {string.Format(ChatUserSQLQueryHelper.CountQuery, filter)}";

            var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var queryResult = await db.QueryMultipleAsync(command);

                var entities = await queryResult.ReadAsync<ChatUserModel>();
                var totalCount = await queryResult.ReadSingleAsync<int>();

                var totalPages = (int)Math.Ceiling(totalCount / (double)listParameters.PageSize);

                var result = new PaginatedResult<ChatUserModel, ChatUserSort>
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

        public async Task<ChatUserModel> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", id);

            var query = string.Format(ChatUserSQLQueryHelper.GetOneQuery, _queryHelper.Where("[ID] = @ID"));

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                var result = await db.QuerySingleAsync<ChatUserModel>(command);

                return result;
            }
        }

        public async Task<bool> UpdateAsync(UpdateChatUserModel item, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", item.ID);
            var changeParameters = _queryHelper.CreateQueryUpdateParameters(item, parameters);
            var query = string.Format(ChatUserSQLQueryHelper.UpdateQuery, changeParameters);

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);

                var result = await db.ExecuteAsync(command);

                return result == 1;
            }
        }
    }
}
