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
    public class IdentityRepositoryTests
    {
        private readonly IdentityRepository _identityRepository;
        private readonly DatabaseFixture _databaseFixture;
        private readonly IdentityModelFixtureHelper _identityModelFixtureHelper;
        private readonly ChatUserFixtureHelper _chatUserFixtureHelper;

        public IdentityRepositoryTests()
        {
            _databaseFixture = new DatabaseFixture();
            _databaseFixture.EnsureCreated(true);

            _identityModelFixtureHelper = new IdentityModelFixtureHelper();
            _chatUserFixtureHelper = new ChatUserFixtureHelper(_databaseFixture.dbOptions);

            var optionsMock = new Mock<IOptions<DatabaseOptions>>();
            var loggerMock = new Mock<ILogger<IdentityRepository>>();
            optionsMock.Setup(x => x.Value)
            .Returns(_databaseFixture.dbOptions);

            _identityRepository = new IdentityRepository(optionsMock.Object, loggerMock.Object);
        }

        [Fact]
        public async void GetAsync_GetExistingIdentityFromDb_ReturnsIdentityModel()
        {
            //Arrange
            CancellationToken token = default;
            var identityModel = _identityModelFixtureHelper.CreateRandomIdentity();
            var chatUserID = await _chatUserFixtureHelper.AddRandomChatUserToDb(token);
            var createModel = new CreateIdentityModel() 
            {
                ID = identityModel.ID,
                Email = identityModel.Email,
                UserTag = identityModel.UserTag,
                PasswordHash = identityModel.PasswordHash,
                PasswordKey = identityModel.PasswordKey,
                UserID = chatUserID
            };

            await _identityRepository.CreateAsync(createModel,token);

            //Act
            var actual = await _identityRepository.GetAsync(createModel.ID, token);
            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actual.Should().NotBeNull();
            actual.ID.Should().Be(identityModel.ID);
        }

        [Fact]
        public async void GetAsync_GetInexistentIdetityFromDb_ReturnsNull()
        {
            //Arrange
            CancellationToken token = default;

            //Act
            var actual = await _identityRepository.GetAsync(Guid.NewGuid(), token);

            //Assert
            actual.Should().BeNull();
        }

        [Fact]
        public async void DeleteAsync_DeleteExistingIdetityFromDb_ReturnsDeletionStatusDeleted()
        {
            //Arrange
            CancellationToken token = default;
            var identityModel = _identityModelFixtureHelper.CreateRandomIdentity();
            var chatUserID = await _chatUserFixtureHelper.AddRandomChatUserToDb(token);
            identityModel.UserID = chatUserID;
            var createModel = new CreateIdentityModel()
            {
                ID = identityModel.ID,
                Email = identityModel.Email,
                UserTag = identityModel.UserTag,
                PasswordHash = identityModel.PasswordHash,
                PasswordKey = identityModel.PasswordKey,
                UserID = chatUserID
            };

            await _identityRepository.CreateAsync(createModel, token);

            //Act
            var actual = await _identityRepository.DeleteAsync(identityModel.ID, token);
            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actual.Should().Be(DeletionStatus.Deleted);

        }

        [Fact]
        public async void DeleteAsync_DeleteInexistentIdentityFromDb_DeletionStatusIsNotExisted()
        {
            // Arrange
            CancellationToken token = default;

            // Act
            var actual = await _identityRepository.DeleteAsync(Guid.NewGuid(), token);

            // Assert
            actual.Should().Be(DeletionStatus.NotExisted);
        }

        [Fact]
        public async void UpdateAsync_UpdateExistingIdentityFromDb_ReturnsTrue()
        {
            //Arrange
            CancellationToken token = default;
            var identityModel = _identityModelFixtureHelper.CreateRandomIdentity();
            var chatUserID = await _chatUserFixtureHelper.AddRandomChatUserToDb(token);
            var createModel = new CreateIdentityModel()
            {
                ID = identityModel.ID,
                Email = identityModel.Email,
                UserTag = identityModel.UserTag,
                PasswordHash = identityModel.PasswordHash,
                PasswordKey = identityModel.PasswordKey,
                UserID = chatUserID
            };
           
            await _identityRepository.CreateAsync(createModel, token);

            var updateModel = new UpdateIdentityModel()
            {
                ID = identityModel.ID,
                Email = "New",
                UserTag = "New",
                PasswordHash = "New",
                PasswordKey = "New"
            };

            //Act
            var actual = await _identityRepository.UpdateAsync(updateModel, token);
            var actualResult = await _identityRepository.GetAsync(identityModel.ID, token);

            var expected = new IdentityModel()
            {
                ID = identityModel.ID,
                Email = updateModel.Email,
                UserTag = updateModel.UserTag,
                PasswordHash = updateModel.PasswordHash,
                PasswordKey = updateModel.PasswordKey,
                UserID = actualResult.UserID
            };

            //Assert
            actual.Should().BeTrue();
            actualResult.Should().BeEquivalentTo(expected);

        }

        [Fact]
        public async void UpdateAsync_UpdateUnxistentIdentityInDb_ReturnsFalse()
        {
            //Arrange
            CancellationToken token = default;
            var updateModel = new UpdateIdentityModel()
            {
                ID = Guid.NewGuid(),
                Email = "New",
                UserTag = "New",
                PasswordHash = "New",
                PasswordKey = "New",
            };

            //Act
            var actual = await _identityRepository.UpdateAsync(updateModel, token);
            await _databaseFixture.ClearDatabaseAsync();
            //Assert
            Assert.False(actual);
        }
    }
}