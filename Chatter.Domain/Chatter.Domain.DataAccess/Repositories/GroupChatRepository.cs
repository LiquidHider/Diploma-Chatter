using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.DbOptions;
using Chatter.Domain.DataAccess.Interfaces;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.DataAccess.Models.Parameters;
using Chatter.Domain.DataAccess.Utilities;
using Dapper;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;

namespace Chatter.Domain.DataAccess.Repositories
{
    internal class GroupChatRepository : IGroupChatRepository
    {

        private readonly DatabaseOptions _dbOptions;
        private readonly GroupChatSQLQueryHelper _queryHelper;
        private readonly ILogger<GroupChatRepository> _logger;

        public GroupChatRepository(DatabaseOptions dbOptions, ILogger<GroupChatRepository> logger)
        {
            _dbOptions = dbOptions;
            _queryHelper = new GroupChatSQLQueryHelper();
            _logger = logger;
        }

        public async Task CreateAsync(GroupChat item, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", item.ID);
            parameters.Add("@Name", item.Name);
            parameters.Add("@Description", item.Description);

            var query = GroupChatSQLQueryHelper.CreateQuery;
            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var command = new CommandDefinition(query, parameters);
                var queryResult = db.ExecuteAsync(command);
            }
        }

        public async Task<DeletionStatus> DeleteAsync(GroupChat item, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID",item.ID);
            int deletedRows = 0;

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection)) 
            {
                var command = new CommandDefinition(GroupChatSQLQueryHelper.DeleteQuery,parameters, cancellationToken: cancellationToken);
                deletedRows = await db.ExecuteAsync(command);
            }

            if (deletedRows == 0)
            {
                return DeletionStatus.NotExisted;
            }

            return DeletionStatus.Deleted;
        }

        public async Task<IList<GroupChat>> ListAsync(GroupChatListParameters listParameters, CancellationToken cancellationToken)
        {
            if (listParameters == null) 
            {
                throw new ArgumentException(nameof(listParameters));
            }

            var parameters = new DynamicParameters();
            var sortBy = _queryHelper.Where(listParameters.SortBy.ToString());
            var sortOrder = _queryHelper.OrderBy(listParameters.SortOrder.ToString());

            var query = string.Format(GroupChatSQLQueryHelper.ListQuery, sortBy, sortOrder);

            var command = new CommandDefinition(query,parameters, cancellationToken: cancellationToken);

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection)) 
            {
                var result = await db.QueryAsync<GroupChat>(command);

                return result.ToList();
            }
        }

        public async Task<GroupChat> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", id);

            var query = string.Format(GroupChatSQLQueryHelper.GetOneQuery, _queryHelper.Where("[ID] = @ID"));

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                var result = await db.QuerySingleAsync<GroupChat>(query);

                return result;
            }
        }

        public async Task<bool> UpdateAsync(UpdateGroupChat item, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            
            var changeParameters = _queryHelper.CreateQueryUpdateParameters(item, parameters);
            var query = string.Format(GroupChatSQLQueryHelper.UpdateQuery, changeParameters);

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection)) 
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                
                var result = await db.ExecuteAsync(command);

                return result == 1;
            }
        }
    }
}
