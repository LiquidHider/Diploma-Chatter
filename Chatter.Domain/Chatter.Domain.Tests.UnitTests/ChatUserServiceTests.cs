using AutoFixture;
using Chatter.Domain.BusinessLogic.MessagesContainers;
using Chatter.Domain.BusinessLogic.Models;
using Chatter.Domain.BusinessLogic.Models.Abstract;
using Chatter.Domain.BusinessLogic.Models.Create;
using Chatter.Domain.BusinessLogic.Models.Update;
using Chatter.Domain.BusinessLogic.Services;
using Chatter.Domain.DataAccess.Interfaces;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.Tests.Common;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;

namespace Chatter.Domain.Tests.UnitTests
{
    public class ChatUserServiceTests
    {
        private readonly ChatUserService _chatUserService;

        private readonly Fixture _fixture;

        private readonly Mock<IChatUserRepository> _chatUserRepositoryMock;

        private readonly Mock<ILogger<ChatUserService>> _loggerMock;

        private readonly ChatUserFixtureHelper _chatUserFixtureHelper;

        public ChatUserServiceTests()
        {
            _fixture = new Fixture();
            _chatUserFixtureHelper = new ChatUserFixtureHelper();
           _chatUserRepositoryMock = new Mock<IChatUserRepository>();
            _loggerMock = new Mock<ILogger<ChatUserService>>();
            _chatUserService = new ChatUserService(_chatUserRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async void GetUserAsync_GetExistingUserWithContacts_ReturnsServiceResultWithChatUser() 
        {
            //Arrange
            CancellationToken token = new CancellationToken();

            var resultFromDb = _chatUserFixtureHelper.CreateRandomChatUser();
            var contacts = _fixture.Create<IList<Guid>>();
            _chatUserRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), token))
                .Returns(Task.FromResult(resultFromDb));

            _chatUserRepositoryMock.Setup(x => x.GetUserContactsAsync(It.IsAny<Guid>(), token))
                .Returns(Task.FromResult(contacts));

            //Act

            var actual = await _chatUserService.GetUserAsync(resultFromDb.ID, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Contacts.Count.Should().Be(contacts.Count);
            actual.Value.ID.Should().Be(resultFromDb.ID);
        }
        [Fact]
        public async void GetUserAsync_GetExistingUserWithNoContacts_ReturnsServiceResultWithChatUser()
        {
            //Arrange
            CancellationToken token = new CancellationToken();

            var resultFromDb = _chatUserFixtureHelper.CreateRandomChatUser();
            _chatUserRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), token))
                .Returns(Task.FromResult(resultFromDb));

            //Act

            var actual = await _chatUserService.GetUserAsync(resultFromDb.ID, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Contacts.Should().NotBeNull();
            actual.Value.Contacts.Should().BeEmpty();
            actual.Value.ID.Should().Be(resultFromDb.ID);
        }
        [Fact]
        public async void GetUserAsync_GetInexistentUser_ReturnsServiceResultWithBusinessError()
        {
            //Arrange
            CancellationToken token = new CancellationToken();

            //Act
            var actual = await _chatUserService.GetUserAsync(Guid.NewGuid(), token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(BusinessLogic.Enums.ErrorType.BusinessError);
            actual.Error.Message.Should().Be(ChatUserServiceMessagesContainer.UserNotExist);
        }

        [Fact]
        public async void UpdateUserAsync_UpdateExistingUser_ReturnsServiceResultWithUpdatedUserID()
        {
            //Arrange
            CancellationToken token = default;
            var userFromDb = _chatUserFixtureHelper.CreateRandomChatUser();
            var updateModel = _fixture.Create<UpdateChatUser>();
            _chatUserRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), token))
                .Returns(Task.FromResult(userFromDb));

            _chatUserRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<UpdateChatUserModel>(), token))
                .Returns(Task.FromResult(true));

            //Act
            var actual = await _chatUserService.UpdateUserAsync(updateModel, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async void UpdateUserAsync_UpdateInexistentUser_ReturnsServiceResultWithBusinessError()
        {
            //Arrange
            CancellationToken token = default;
            var updateModel = _fixture.Create<UpdateChatUser>();

            //Act
            var actual = await _chatUserService.UpdateUserAsync(updateModel, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(BusinessLogic.Enums.ErrorType.BusinessError);
            actual.Error.Message.Should().Be(ChatUserServiceMessagesContainer.UserNotExist);
        }

        [Fact]
        public async void UpdateUserAsync_UnsuccessfullyUpdateExistingUser_ReturnsServiceResultWithDataError()
        {
            //Arrange
            CancellationToken token = default;
            var userFromDb = _chatUserFixtureHelper.CreateRandomChatUser();

            var updateModel = _fixture.Create<UpdateChatUser>();

            _chatUserRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), token))
                .Returns(Task.FromResult(userFromDb));

            _chatUserRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<UpdateChatUserModel>(), token))
                .Returns(Task.FromResult(false));

            //Act
            var actual = await _chatUserService.UpdateUserAsync(updateModel, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(BusinessLogic.Enums.ErrorType.DataError);
        }

        [Fact]
        public async void CreateNewUserAsync_CreateNewUser_ReturnsServiceResultWithUser() 
        {
            //Arrange
            CancellationToken token = default;
            var createModel = _fixture.Create<CreateChatUser>();

            //Act
            var actual = await _chatUserService.CreateNewUserAsync(createModel, token);

            //Arrange
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.ID.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async void CreateNewUserAsync_TryToCreateUserWithNullCreateModel_ReturnsServiceResultWithException() 
        {
            //Arrange
            CancellationToken token = default;

            //Act
            var actual = await _chatUserService.CreateNewUserAsync(null, token);

            //Arrange
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(BusinessLogic.Enums.ErrorType.Exception);
        }

        [Fact]
        public async void DeleteUserAsync_DeleteExistingUser_ReturnsServiceResultWithDeletedUserID()
        {
            //Arrange
            CancellationToken token = default;
            var userId = Guid.NewGuid();

            _chatUserRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<Guid>(), token))
                .Returns(Task.FromResult(Domain.Common.Enums.DeletionStatus.Deleted));

            //Act
            var actual = await _chatUserService.DeleteUserAsync(userId, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Should().NotBe(Guid.Empty);
        }


        [Fact]
        public async void DeleteUserAsync_DeleteInexitentUser_ReturnsServiceResultWithBuinessError()
        {
            //Arrange
            CancellationToken token = default;

            _chatUserRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<Guid>(), token))
                .Returns(Task.FromResult(Domain.Common.Enums.DeletionStatus.NotExisted));

            //Act
            var actual = await _chatUserService.DeleteUserAsync(Guid.NewGuid(), token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(BusinessLogic.Enums.ErrorType.BusinessError);
            actual.Error.Message.Should().Be(ChatUserServiceMessagesContainer.UserNotExist);
        }

        [Fact]
        public async void BlockUserAsync_BlockExistingUserWithUnblockDate_ReturnsServiceResultWithBlockedUserID()
        {
            //Arrange
            CancellationToken token = default;
            var userFromDb = _chatUserFixtureHelper.CreateRandomChatUser();
            userFromDb.IsBlocked = false;
            userFromDb.BlockedUntil = null;
            var blockModel = _fixture.Create<BlockUser>();


            _chatUserRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), token))
                .Returns(Task.FromResult(userFromDb));

            _chatUserRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<UpdateChatUserModel>(), token))
                .Returns(Task.FromResult(true));

            //Act
            var actual = await _chatUserService.BlockUserAsync(blockModel, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async void BlockUserAsync_BlockExistingUserWithNoUnblockDate_ReturnsServiceResultWithBlockedUserID()
        {
            //Arrange
            CancellationToken token = default;
            var userFromDb = _chatUserFixtureHelper.CreateRandomChatUser();
            userFromDb.IsBlocked = false;
            userFromDb.BlockedUntil = null;
            var blockModel = _fixture.Create<BlockUser>();
            blockModel.BlockedUntilUtc = null;

            _chatUserRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), token))
                .Returns(Task.FromResult(userFromDb));

            _chatUserRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<UpdateChatUserModel>(), token))
                .Returns(Task.FromResult(true));

            //Act
            var actual = await _chatUserService.BlockUserAsync(blockModel, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async void BlockUserAsync_BlockAlreadyBlockedExistingUserWithUnblockDate_ReturnsServiceResultWithBusinessError()
        {
            //Arrange
            CancellationToken token = default;
            var userFromDb = _chatUserFixtureHelper.CreateRandomChatUser();
            userFromDb.IsBlocked = true;
            userFromDb.BlockedUntil = _fixture.Create<DateTime>();
            var blockModel = _fixture.Create<BlockUser>();


            _chatUserRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), token))
                .Returns(Task.FromResult(userFromDb));

            //Act
            var actual = await _chatUserService.BlockUserAsync(blockModel, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(BusinessLogic.Enums.ErrorType.BusinessError);
            actual.Error.Message.Should().Be(ChatUserServiceMessagesContainer.UserAlreadyBlocked);
        }

        [Fact]
        public async void BlockUserAsync_BlockInexistentUser_ReturnsServiceResultWithBusinessError()
        {
            //Arrange
            CancellationToken token = default;
            var blockModel = _fixture.Create<BlockUser>();

            //Act
            var actual = await _chatUserService.BlockUserAsync(blockModel, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(BusinessLogic.Enums.ErrorType.BusinessError);
            actual.Error.Message.Should().Be(ChatUserServiceMessagesContainer.UserNotExist);
        }


        [Fact]
        public async void UnblockUserAsync_UnblockExistingUser_ReturnsServiceResultWithUnblockedUserID()
        {
            //Arrange
            CancellationToken token = default;
            var userFromDb = _chatUserFixtureHelper.CreateRandomChatUser();

            _chatUserRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), token))
                .Returns(Task.FromResult(userFromDb));
            _chatUserRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<UpdateChatUserModel>(), token))
                .Returns(Task.FromResult(true));

            //Act
            var actual = await _chatUserService.UnblockUserAsync(userFromDb.ID, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async void UnblockUserAsync_UnblockNotBlockedExistingUser_ReturnsServiceResultWithBusinessError()
        {
            //Arrange
            CancellationToken token = default;
            var userFromDb = _chatUserFixtureHelper.CreateRandomChatUser();
            userFromDb.IsBlocked = false;
            userFromDb.BlockedUntil = null;

            _chatUserRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), token))
                .Returns(Task.FromResult(userFromDb));

            //Act
            var actual = await _chatUserService.UnblockUserAsync(userFromDb.ID, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(BusinessLogic.Enums.ErrorType.BusinessError);
            actual.Error.Message.Should().Be(ChatUserServiceMessagesContainer.UserIsNotBlocked);
        }

        [Fact]
        public async void UnblockUserAsync_UnblockInexistentUser_ReturnsServiceResultWithBusinessError()
        {
            //Arrange
            CancellationToken token = default;

            //Act
            var actual = await _chatUserService.UnblockUserAsync(Guid.NewGuid(), token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(BusinessLogic.Enums.ErrorType.BusinessError);
            actual.Error.Message.Should().Be(ChatUserServiceMessagesContainer.UserNotExist);
        }

        [Fact]
        public async void UnblockUserAsync_UnsuccessfullyUnblockNotBlockedExistingUser_ReturnsServiceResultWithDataError()
        {
            //Arrange
            CancellationToken token = default;
            var userFromDb = _chatUserFixtureHelper.CreateRandomChatUser();
            userFromDb.IsBlocked = true;

            _chatUserRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), token))
                .Returns(Task.FromResult(userFromDb));
            _chatUserRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<UpdateChatUserModel>(), token))
                .Returns(Task.FromResult(false));
            //Act
            var actual = await _chatUserService.UnblockUserAsync(userFromDb.ID, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(BusinessLogic.Enums.ErrorType.DataError);
            actual.Error.Message.Should().Be(ChatUserServiceMessagesContainer.BadRequestError);
        }
    }
}
