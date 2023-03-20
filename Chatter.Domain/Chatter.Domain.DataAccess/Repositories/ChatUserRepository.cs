﻿using Chatter.Domain.Common.Enums;
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
    internal class ChatUserRepository : IChatUserRepository
    {
        private readonly DatabaseOptions _dbOptions;
        private readonly ChatUserSQLQueryHelper _queryHelper;
        private readonly ILogger<ChatUserRepository> _logger;

        public ChatUserRepository(DatabaseOptions dbOptions, ILogger<ChatUserRepository> logger)
        {
            _queryHelper = new ChatUserSQLQueryHelper();
            _logger = logger;
            _dbOptions = dbOptions;
        }

        public async Task CreateAsync(ChatUser item, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", item.ID);
            parameters.Add("@LastName", item.LastName);
            parameters.Add("@FirstName", item.FirstName);
            parameters.Add("@Patronymic", item.Patronymic);
            parameters.Add("@UserTag", item.UserTag);
            parameters.Add("@Email", item.Email);
            parameters.Add("@UniversityName", item.UniversityName);
            parameters.Add("@UniversityFaculty", item.UniversityFaculty);
            parameters.Add("@JoinedUtc", item.JoinedUtc);
            parameters.Add("@LastActiveUtc", item.LastActiveUtc);
            parameters.Add("@IsBlocked", item.IsBlocked);
            parameters.Add("@BlockedUntilUtc", item.BlockedUntilUtc);

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var commandDefinition = new CommandDefinition(ChatUserSQLQueryHelper.CreateQuery, parameters, cancellationToken: cancellationToken);
                _logger.LogInformation($"Created query: {commandDefinition.CommandText}");
                var createQuery = await db.ExecuteAsync(commandDefinition);
            }
        }

        public async Task<DeletionStatus> DeleteAsync(ChatUser item, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", item.ID);
            int deletedRows = 0;

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var command = new CommandDefinition(ChatMessageSQLQueryHelper.DeleteQuery, parameters, cancellationToken: cancellationToken);
                deletedRows = await db.ExecuteAsync(command);
            }

            if (deletedRows == 0)
            {
                return DeletionStatus.NotExisted;
            }

            return DeletionStatus.Deleted;
        }

        public async Task<IList<ChatUser>> ListAsync(ChatUserListParameters listParameters, CancellationToken cancellationToken)
        {
            if (listParameters == null)
            {
                throw new ArgumentException(nameof(listParameters));
            }

            var parameters = new DynamicParameters();
            var sortBy = _queryHelper.Where(listParameters.SortBy.ToString());
            var sortOrder = _queryHelper.OrderBy(listParameters.SortOrder.ToString());

            var query = string.Format(ChatUserSQLQueryHelper.ListQuery, sortBy, sortOrder);

            var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var result = await db.QueryAsync<ChatUser>(command);

                return result.ToList();
            }
        }

        public async Task<ChatUser> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", id);

            var query = string.Format(ChatUserSQLQueryHelper.GetOneQuery, _queryHelper.Where("[ID] = @ID"));

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                var result = await db.QuerySingleAsync<ChatUser>(query);

                return result;
            }
        }

        public async Task<bool> UpdateAsync(ChatUser item, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();

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
