using Chatter.Security.DataAccess.DbOptions;
using Chatter.Security.Common.Enums;
using Chatter.Security.DataAccess.Interfaces;
using Chatter.Security.DataAccess.Models;
using Chatter.Security.DataAccess.SQLQueryHelpers;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;

namespace Chatter.Security.DataAccess.Repositories
{
    public class IdentityRepository : IIdentityRepository
    {
        private readonly DatabaseOptions _dbOptions;
        private readonly ILogger<IdentityRepository> _logger;
        private readonly IdentitySQLQueryHelper _queryHelper;

        public IdentityRepository(IOptions<DatabaseOptions> dbOptions, ILogger<IdentityRepository> logger)
        {
            _dbOptions = dbOptions.Value;
            _logger = logger;
            _queryHelper = new IdentitySQLQueryHelper();
        }

        public async Task CreateAsync(CreateIdentityModel createModel, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateAsync : {@Details}", new { Class = nameof(IdentityRepository), Method = nameof(CreateAsync) });
            var parameters = new DynamicParameters();
            parameters.Add("@ID", createModel.ID);
            parameters.Add("@Email", createModel.Email);
            parameters.Add("@UserTag", createModel.UserTag);
            parameters.Add("@PasswordHash", createModel.PasswordHash);
            parameters.Add("@PasswordKey", createModel.PasswordKey);
            parameters.Add("@UserID", createModel.UserID);

            var query = IdentitySQLQueryHelper.CreateQuery;
            var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection)) 
            {
                var createResult = await db.ExecuteAsync(command);
            }
        }

        public async Task<DeletionStatus> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("DeleteAsync : {@Details}", new { Class = nameof(IdentityRepository), Method = nameof(DeleteAsync) });
            var parameters = new DynamicParameters();
            parameters.Add("@ID", id);

            var query = IdentitySQLQueryHelper.DeleteQuery;

            var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
            var deletedRows = 0;
            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                deletedRows = await db.ExecuteAsync(command);
            }
            if (deletedRows == 0)
            {
                return DeletionStatus.NotExisted;
            }

            return DeletionStatus.Deleted;
        }
    

        public async Task<IdentityModel> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", id);

            var query = IdentitySQLQueryHelper.GetOneQuery;

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                var result = await db.QueryFirstOrDefaultAsync<IdentityModel>(command);

                return result;
            }
        }

        public async Task<bool> UpdateAsync(UpdateIdentityModel updateModel, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ID", updateModel.ID);
            var changeParameters = _queryHelper.CreateQueryUpdateParameters(updateModel, parameters);
            var query = string.Format(IdentitySQLQueryHelper.UpdateQuery, changeParameters);

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);

                var result = await db.ExecuteAsync(command);

                return result == 1;
            }
        }
    }
}
