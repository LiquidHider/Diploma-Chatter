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
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly ChatMessageSQLQueryHelper _queryHelper;
        private readonly DatabaseOptions _dbOptions;
        private readonly ILogger<ChatMessageRepository> _logger;

        public ChatMessageRepository(IOptions<DatabaseOptions> dbOptions, ILogger<ChatMessageRepository> logger)
        {
            _queryHelper = new ChatMessageSQLQueryHelper();
            _dbOptions = dbOptions?.Value;
            _logger = logger;
        }

        public async Task CreateAsync(ChatMessageModel item, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID",item.ID);
            parameters.Add("@Body", item.Body);
            parameters.Add("@IsEdited", item.IsEdited);
            parameters.Add("@Sent", item.Sent);
            parameters.Add("@IsRead", item.IsRead);
            parameters.Add("@Sender", item.Sender);
            _queryHelper.DefineRecipientToQuery(item, parameters);

            var query = ChatMessageSQLQueryHelper.CreateQuery;

            var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection)) 
            {
                var createQuery = await db.ExecuteAsync(command);
            }
        }

        public async Task<DeletionStatus> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", id);

            var deletedRows = 0;

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

        public async Task<IList<ChatMessageModel>> ListAsync(ChatMessageListParameters listParameters, CancellationToken cancellationToken)
        {
            if (listParameters == null)
            {
                throw new ArgumentException(nameof(listParameters));
            }

            var parameters = new DynamicParameters();
            parameters.Add("@Recipient", listParameters.Recipient);
            var targetTable = listParameters.RecipientIsGroup ? "RecipientGroup" : "RecipientUser";

            var filter = _queryHelper.Where($"{targetTable} = @Recipient");

            var sortOrder = _queryHelper.OrderBy(listParameters.SortOrder.ToString(), listParameters.SortBy.ToString());

            var query = string.Format(ChatMessageSQLQueryHelper.ListQuery, filter, sortOrder);

            var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var result = await db.QueryAsync<ChatMessageModel>(command);

                return result.ToList();
            }
        }

        public async Task<ChatMessageModel> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            var query = string.Format(ChatMessageSQLQueryHelper.GetOneQuery, _queryHelper.Where("[ID] = @Id"));

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                var result = await db.QuerySingleAsync<ChatMessageModel>(command);

                return result;
            }
        }

        public async Task<bool> UpdateAsync(UpdateChatMessageModel item, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", item.ID);
            var changeParameters = _queryHelper.CreateQueryUpdateParameters(item, parameters);
            
            var query = string.Format(ChatMessageSQLQueryHelper.UpdateQuery, changeParameters);

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);

                var result = await db.ExecuteAsync(command);

                return result == 1;
            }
        }
    }
}
