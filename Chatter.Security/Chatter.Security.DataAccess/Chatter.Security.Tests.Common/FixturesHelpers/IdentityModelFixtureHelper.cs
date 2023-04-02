using AutoFixture;
using Chatter.Security.DataAccess.Models;

namespace Chatter.Security.Tests.Common.FixturesHelpers
{
    public class IdentityModelFixtureHelper
    {
        private readonly Fixture _fixture;

        public IdentityModelFixtureHelper()
        {
            _fixture = new Fixture();
        }

        public IdentityModel CreateRandomIdentity() 
        {
            return new IdentityModel() 
            {
                ID = Guid.NewGuid(),
                Email = _fixture.Create<string>().Substring(0, 20),
                UserTag = _fixture.Create<string>().Substring(0, 20),
                PasswordHash = _fixture.Create<string>(),
                PasswordKey = _fixture.Create<string>(),
                UserID = Guid.NewGuid()
            };
        }
    }
}
