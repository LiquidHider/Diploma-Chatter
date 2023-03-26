using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.DbOptions;
using Chatter.Domain.DataAccess.Interfaces;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.DataAccess.Models.Parameters;
using Chatter.Domain.DataAccess.Utilities;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;

namespace Chatter.Domain.DataAccess.Repositories
{
    public class GroupChatRepository : IGroupChatRepository
    {

        private readonly DatabaseOptions _dbOptions;
        private readonly GroupChatSQLQueryHelper _queryHelper;
        private readonly ILogger<GroupChatRepository> _logger;

        public GroupChatRepository(IOptions<DatabaseOptions> dbOptions, ILogger<GroupChatRepository> logger)
        {
            _dbOptions = dbOptions?.Value;
            _queryHelper = new GroupChatSQLQueryHelper();
            _logger = logger;
        }

        public async Task CreateGroupChatAsync(GroupChatModel item, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", item.ID);
            parameters.Add("@Name", item.Name);
            parameters.Add("@Description", item.Description);

            var query = GroupChatSQLQueryHelper.CreateQuery;
            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                var queryResult = await db.ExecuteAsync(command);
            }
        }

        public async Task<DeletionStatus> DeleteGroupChatAsync(Guid id, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", id);
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

        public async Task<IList<GroupChatModel>> ListGroupChatsAsync(GroupChatListParameters listParameters, CancellationToken cancellationToken)
        {
            if (listParameters == null) 
            {
                throw new ArgumentException(nameof(listParameters));
            }

            var parameters = new DynamicParameters();
            var filterPart = string.Empty;
       

            if (listParameters.UserId != null)
            {
                var groupIdSelectSubQuery = $@"[ID] IN (SELECT [GroupId] 
                                          FROM 
                                          [dbo].[UserJoinedGroups]
                                          WHERE [UserID] = '{listParameters.UserId}')";
                filterPart = _queryHelper.Where(groupIdSelectSubQuery);
            }

            var sortOrder = _queryHelper.OrderBy(listParameters.SortOrder.ToString(), listParameters.SortBy.ToString());

            var query = string.Format(GroupChatSQLQueryHelper.ListQuery, filterPart, sortOrder);

            var command = new CommandDefinition(query,parameters, cancellationToken: cancellationToken);

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection)) 
            {
                var result = await db.QueryAsync<GroupChatModel>(command);

                return result.ToList();
            }
        }

        public async Task<GroupChatModel> GetGroupChatAsync(Guid id, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", id);

            var query = string.Format(GroupChatSQLQueryHelper.GetOneQuery, _queryHelper.Where("[ID] = @ID"));

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                var result = await db.QueryFirstOrDefaultAsync<GroupChatModel>(command);

                return result;
            }
        }

        public async Task<bool> UpdateAsync(UpdateGroupChatModel item, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", item.ID);
            var changeParameters = _queryHelper.CreateQueryUpdateParameters(item, parameters);
            var query = string.Format(GroupChatSQLQueryHelper.UpdateQuery, changeParameters);

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection)) 
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                
                var result = await db.ExecuteAsync(command);

                return result == 1;
            }
        }

        public async Task AddGroupParticipantAsync(GroupUserModel groupUser, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", groupUser.ID);
            parameters.Add("@GroupID", groupUser.GroupID);
            parameters.Add("@UserID", groupUser.UserID);
            parameters.Add("@UserRole", groupUser.UserRole);

            var query = GroupChatSQLQueryHelper.CreateGroupParticipantQuery;
            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                var queryResult = await db.ExecuteAsync(command);
            }
        }

        public async Task<DeletionStatus> DeleteGroupParticipantAsync(Guid participantId, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", participantId);
            int deletedRows = 0;

            var query = GroupChatSQLQueryHelper.DeleteGroupParticipantQuery;
            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                deletedRows = await db.ExecuteAsync(command);
            }

            if (deletedRows == 0)
            {
                return DeletionStatus.NotExisted;
            }

            return DeletionStatus.Deleted;
        }

        public async Task<List<GroupUserModel>> ListGroupParticipantsAsync(GroupParticipantsListParameters listParameters, CancellationToken cancellationToken) 
        {
            var parameters = new DynamicParameters();

            var filterPart = _queryHelper.Where($"GroupID = \'{listParameters.GroupID}\'");
            
            if (listParameters.ShowBlocked != null && listParameters.ShowBlocked == false) 
            {
                var dontShowBlockedParticipantsSubquery = $@"
                    [UserID] NOT IN (SELECT [UserID] FROM [dbo].[BlockedGroupChatUsers] WHERE [GroupID] = '{listParameters.GroupID}' )";
                filterPart += $"AND {dontShowBlockedParticipantsSubquery}";
            }

            var sortBy = _queryHelper.OrderBy(listParameters.SortOrder.ToString(), listParameters.SortBy.ToString());

            var query = string.Format(GroupChatSQLQueryHelper.ListGroupParticipantsQuery, filterPart, sortBy);

            var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var result = await db.QueryAsync<GroupUserModel>(command);

                return result.ToList();
            }
        }

        public async Task SetGroupUserAsBlockedAsync(GroupChatBlockModel blockModel, CancellationToken cancellationToken) 
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", blockModel.ID);
            parameters.Add("@GroupID", blockModel.GroupID);
            parameters.Add("@UserID", blockModel.UserID);
            parameters.Add("@BlockedUntil", blockModel.BlockedUntil);

            var query = GroupChatSQLQueryHelper.CreateBlockedGroupUserQuery;
            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                var queryResult = await db.ExecuteAsync(command);
            }
        }

        public async Task<DeletionStatus> DeleteGroupUserFromBlockedUsersAsync(Guid blockId, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", blockId);
            int deletedRows = 0;

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var command = new CommandDefinition(GroupChatSQLQueryHelper.DeleteBlockedGroupUserQuery, parameters, cancellationToken: cancellationToken);
                deletedRows = await db.ExecuteAsync(command);
            }

            if (deletedRows == 0)
            {
                return DeletionStatus.NotExisted;
            }

            return DeletionStatus.Deleted;
        }

        public async Task<List<GroupChatBlockModel>> ListBlockedUsers(Guid groupId, CancellationToken cancellationToken) 
        {
            var parameters = new DynamicParameters();

            var filterPart = _queryHelper.Where($"GroupID = \'{groupId}\'");
            var query = string.Format(GroupChatSQLQueryHelper.ListBlockedGroupUsersQuery, filterPart);

            var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var result = await db.QueryAsync<GroupChatBlockModel>(command);

                return result.ToList();
            }
        }
    }
}
