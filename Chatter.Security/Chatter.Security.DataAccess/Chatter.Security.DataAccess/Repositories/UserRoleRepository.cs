using Chatter.Security.Common.Enums;
using Chatter.Security.DataAccess.DbOptions;
using Chatter.Security.DataAccess.Interfaces;
using Chatter.Security.DataAccess.SQLQueryHelpers;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;

namespace Chatter.Security.DataAccess.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly DatabaseOptions _dbOptions;
        private readonly IdentitySQLQueryHelper _queryHelper;
        private readonly ILogger<UserRoleRepository> _logger;

        public UserRoleRepository(IOptions<DatabaseOptions> dbOptions, ILogger<UserRoleRepository> logger)
        {
            _queryHelper = new IdentitySQLQueryHelper();
            _dbOptions = dbOptions.Value;
            _logger = logger;
        }

        public async Task<Guid> AddRoleToUserAsync(Guid identityId, UserRole userRole, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AddRoleToUserAsync : {@Details}", new { Class = nameof(IdentityRepository), Method = nameof(AddRoleToUserAsync) });
            var parameters = new DynamicParameters();
            var roleId = Guid.NewGuid();
            parameters.Add("@ID", roleId);
            parameters.Add("@IdentityID", identityId);
            parameters.Add("@UserRole", userRole);
            parameters.Add("@UserRoleName", userRole.ToString());

            var query = UserRoleSQLQueryHelper.CreateQuery;

            var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection)) 
            {
               var queryResult = await db.ExecuteAsync(command);
               return roleId;
            }
        }

        public async Task<DeletionStatus> DeleteUserRoleAsync(Guid identityId, UserRole userRole, CancellationToken cancellationToken)
        {
            _logger.LogInformation("DeleteUserRoleAsync : {@Details}", new { Class = nameof(IdentityRepository), Method = nameof(DeleteUserRoleAsync) });
            var parameters = new DynamicParameters();
            parameters.Add("@IdentityID", identityId);
            parameters.Add("@UserRole", userRole);

            var query = UserRoleSQLQueryHelper.DeleteQuery;

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

        public async Task<Guid> GetRoleIdAsync(Guid identityId, UserRole userRole, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@IdentityID", identityId);
            parameters.Add("@UserRole", userRole);

            var query = UserRoleSQLQueryHelper.GetRoleIdQuery;

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);
                var result = await db.QueryFirstOrDefaultAsync<Guid>(command);

                return result;
            }
        }

        public async Task<IList<UserRole>> GetUserRolesAsync(Guid identityId, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@IdentityID", identityId);

            var query = UserRoleSQLQueryHelper.UserRolesQuery;

            var command = new CommandDefinition(query, parameters, cancellationToken: cancellationToken);

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection)) 
            {
               var result = await db.QueryAsync<UserRole>(command);

               return result.ToList();
            }
        }
    }
}
