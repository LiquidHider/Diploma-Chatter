
using AutoFixture;
using Chatter.Security.DataAccess.Models;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
using Chatter.Security.DataAccess.DbOptions;

namespace Chatter.Security.Tests.Common.FixturesHelpers
{
    public class ChatUserFixtureHelper
    {
        private readonly Fixture _fixture;
        private readonly DatabaseOptions _dbOptions;

        public ChatUserFixtureHelper(DatabaseOptions dbOptions)
        {
            _fixture = new Fixture();
            _dbOptions = dbOptions;
        }

        public const string CreateQuery = @"
            INSERT INTO [dbo].[ChatUsers] (
            [ID],
            [LastName],
            [FirstName],
            [Patronymic],
            [UniversityName],
            [UniversityFaculty],
            [JoinedUtc],
            [LastActive],
            [IsBlocked],
            [BlockedUntil])
            VALUES (@ID, @LastName, @FirstName, @Patronymic, @UniversityName, @UniversityFaculty, 
            @JoinedUtc, @LastActiveUtc, @IsBlocked, @BlockedUntilUtc)";

        public async Task<Guid> AddRandomChatUserToDb(CancellationToken token)
        {
            var formatedJoined = _fixture.Create<DateTime>().ToString("yyyy-MM-dd HH:mm:ss.ff");
            var formatedLastActive = _fixture.Create<DateTime>().ToString("yyyy-MM-dd HH:mm:ss.ff");
            var formatedBlockedUntil = _fixture.Create<DateTime>().ToString("yyyy-MM-dd HH:mm:ss.ff");

            var parameters = new DynamicParameters();
            var userId = Guid.NewGuid();

            parameters.Add("@ID", userId);
            parameters.Add("@LastName", _fixture.Create<string>().Substring(0, 20));
            parameters.Add("@FirstName", _fixture.Create<string>().Substring(0, 20));
            parameters.Add("@Patronymic", _fixture.Create<string>().Substring(0, 20));
            parameters.Add("@UniversityName", _fixture.Create<string>().Substring(0, 20));
            parameters.Add("@UniversityFaculty", _fixture.Create<string>().Substring(0, 20));
            parameters.Add("@JoinedUtc", DateTime.Parse(formatedJoined));
            parameters.Add("@LastActiveUtc", DateTime.Parse(formatedLastActive));
            parameters.Add("@IsBlocked", _fixture.Create<bool>());
            parameters.Add("@BlockedUntilUtc", DateTime.Parse(formatedBlockedUntil));

            using (IDbConnection db = new SqlConnection(_dbOptions.ChatterDbConnection))
            {
                var commandDefinition = new CommandDefinition(CreateQuery, parameters, cancellationToken: token);
                await db.ExecuteAsync(commandDefinition);
            }

            return userId;
        }
     }
}
