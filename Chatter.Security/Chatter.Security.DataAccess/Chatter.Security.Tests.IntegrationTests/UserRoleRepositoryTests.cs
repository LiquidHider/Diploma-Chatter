using AutoFixture;
using Chatter.Security.Common.Enums;
using Chatter.Security.DataAccess.DbOptions;
using Chatter.Security.DataAccess.Models;
using Chatter.Security.DataAccess.Repositories;
using Chatter.Security.Tests.Common.FixturesHelpers;
using Chatter.Security.Tests.IntegrationTests.Database;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Chatter.Security.Tests.IntegrationTests
{
    public class UserRoleRepositoryTests
    {
        private readonly UserRoleRepository _userRoleRepository;
        private readonly IdentityRepository _identityRepository;
       
        private readonly Fixture _fixture;
        private readonly DatabaseFixture _databaseFixture;
        private readonly ChatUserFixtureHelper _chatUserFixtureHelper;
        private readonly IdentityModelFixtureHelper _identityFixtureHelper;

        public UserRoleRepositoryTests()
        {
            _databaseFixture = new DatabaseFixture();
            _databaseFixture.EnsureCreated(true);
            _fixture = new Fixture();
            _chatUserFixtureHelper = new ChatUserFixtureHelper(_databaseFixture.dbOptions);
            _identityFixtureHelper = new IdentityModelFixtureHelper();
            var userRoleRepoLoggerMock = new Mock<ILogger<UserRoleRepository>>();
            var identityRepoLoggerMock = new Mock<ILogger<IdentityRepository>>();
            var optionsMock = new Mock<IOptions<DatabaseOptions>>();

            optionsMock.Setup(x => x.Value)
            .Returns(_databaseFixture.dbOptions);

            _userRoleRepository = new UserRoleRepository(optionsMock.Object, userRoleRepoLoggerMock.Object);
            _identityRepository = new IdentityRepository(optionsMock.Object, identityRepoLoggerMock.Object);
        }

        [Fact]
        public async void GetRoleIdAsync_GetExistingUserRoleIdByUserIdAndUserRole_ReturnsUserRoleId() 
        {
            //Arrange
            CancellationToken token = default;

            var roleInsertionResult = await CreateRandomRoleAndAddToDbAsync(token);
            var expected = roleInsertionResult.UserRolesIds.First();

            //Act
            var actual = await _userRoleRepository.GetRoleIdAsync(roleInsertionResult.Identity.ID, roleInsertionResult.UserRoles.First(), token);

            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public async void GetRoleIdAsync_GetInexistentUserRoleIdByUserIdAndUserRole_ReturnsEmptyGuid()
        {
            //Arrange
            CancellationToken token = default;
            var expected = Guid.Empty;

            //Act
            var actual = await _userRoleRepository.GetRoleIdAsync(_fixture.Create<Guid>(), _fixture.Create<UserRole>(), token);

            //Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public async void DeleteUserRoleAsync_RemoveExistingUserRoleIdByUserIdAndUserRole_ReturnsDeletionStatusIsDeleted()
        {
            //Arrange
            CancellationToken token = default;

            var roleInsertionResult = await CreateRandomRoleAndAddToDbAsync(token);
            var expected = DeletionStatus.Deleted;

            //Act
            var actual = await _userRoleRepository.DeleteUserRoleAsync(roleInsertionResult.Identity.ID, roleInsertionResult.UserRoles.First(), token);

            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public async void DeleteUserRoleAsync_RemoveInexistentUserRoleIdByUserIdAndUserRole_ReturnsDeletionStatusNotExisted()
        {
            //Arrange
            CancellationToken token = default;

            var expected = DeletionStatus.NotExisted;

            //Act
            var actual = await _userRoleRepository.DeleteUserRoleAsync(_fixture.Create<Guid>(), _fixture.Create<UserRole>(), token);

            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public async void GetUserRolesAsync_GetAllExistingUserRoles_ReturnsAllUserRoles() 
        {
            //Arrange
            CancellationToken token = default;
            var roleInsertionResult = await CreateRandomRoleAndAddToDbAsync(token, 5);
            await CreateRandomRoleAndAddToDbAsync(token, 5); // create other user roles
            var expected = roleInsertionResult.UserRoles;

            //Act
            var actual = await _userRoleRepository.GetUserRolesAsync(roleInsertionResult.Identity.ID, token);
            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actual.Should().BeEquivalentTo(expected);
        }

        private async Task<UserRoleInsertionResult> CreateRandomRoleAndAddToDbAsync(CancellationToken token, int count = 1) 
        {
            var result = new UserRoleInsertionResult();
            result.ChatUserId = await _chatUserFixtureHelper.AddRandomChatUserToDb(token);
            var identity = _identityFixtureHelper.CreateRandomIdentity();
            identity.UserID = result.ChatUserId;

            result.Identity = identity;

            var mappedIdentity = new CreateIdentityModel()
            {
                ID = identity.ID,
                Email = identity.Email,
                UserTag = identity.UserTag,
                PasswordHash = identity.PasswordHash,
                PasswordKey = identity.PasswordKey,
                UserID = identity.UserID
            };

            await _identityRepository.CreateAsync(mappedIdentity, token);

            for (int i = 0; i < count; i++)
            {
                var userRole = _fixture.Create<UserRole>();
                result.UserRoles.Add(userRole);
                result.UserRolesIds.Add(await _userRoleRepository.AddRoleToUserAsync(identity.ID, userRole, token));
            }

            return result;
        }
        private class UserRoleInsertionResult 
        {
            public Guid ChatUserId { get; set; }

            public IdentityModel Identity { get; set; }

            public List<UserRole> UserRoles { get; set; } = new List<UserRole>();

            public List<Guid> UserRolesIds { get; set; } = new List<Guid>();
        }
    }
}
