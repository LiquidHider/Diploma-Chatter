using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.DbOptions;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.DataAccess.Models.Parameters;
using Chatter.Domain.DataAccess.Repositories;
using Chatter.Domain.IntegrationTests.Database.DatabaseFixture;
using Chatter.Domain.Tests.IntegrationTests.DataAccess.FixturesHelpers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Chatter.Domain.Tests.IntegrationTests.DataAccess
{
    public class ChatMessageRepositoryTests
    {
        private readonly DatabaseFixture _databaseFixture;

        private readonly ChatMessageRepository _chatMessageRepository;
        private readonly GroupChatRepository _groupChatRepository;
        private readonly ChatUserRepository _chatUserRepository;

        private readonly ChatUserFixtureHelper _chatUserFixtureHelper;
        private readonly GroupChatFixtureHelper _groupChatFixtureHelper;
        private readonly ChatMessageFixtureHelper _chatMessageFixtureHelper;
        

        public ChatMessageRepositoryTests()
        {
            _databaseFixture = new DatabaseFixture();
            _databaseFixture.EnsureCreated(true);

            _chatUserFixtureHelper = new ChatUserFixtureHelper();
            _groupChatFixtureHelper = new GroupChatFixtureHelper();
            _chatMessageFixtureHelper = new ChatMessageFixtureHelper();

            var groupChatRepoLoggerMock = new Mock<ILogger<GroupChatRepository>>();
            var chatUserRepoLoggerMock = new Mock<ILogger<ChatUserRepository>>();
            var chatMessageRepoLoggerMock = new Mock<ILogger<ChatMessageRepository>>();

            var optionsMock = new Mock<IOptions<DatabaseOptions>>();
            optionsMock.Setup(x => x.Value)
            .Returns(_databaseFixture.dbOptions);

            _groupChatRepository = new GroupChatRepository(optionsMock.Object, groupChatRepoLoggerMock.Object);
            _chatUserRepository = new ChatUserRepository(optionsMock.Object, chatUserRepoLoggerMock.Object);
            _chatMessageRepository = new ChatMessageRepository(optionsMock.Object, chatMessageRepoLoggerMock.Object);
        }


        [Fact]
        public async void GetAsync_GetExistedMessageFromDb_ReturnsExpectedChatMessage()
        {
            //Arrange
            CancellationToken token = default;
            var sender = _chatUserFixtureHelper.CreateRandomChatUser();
            var userRecipient = _chatUserFixtureHelper.CreateRandomChatUser();
            var expected = _chatMessageFixtureHelper.CreateRandomChatMessage(sender, userRecipient);

            await _chatUserRepository.CreateAsync(sender,token);
            await _chatUserRepository.CreateAsync(userRecipient,token);
            await _chatMessageRepository.CreateAsync(expected, token);

            //Act
            var actual = await _chatMessageRepository.GetAsync(expected.ID, token);
            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(expected);
        }


        [Fact]
        public async void GetAsync_GetInexistentMessageInDb_ReturnsNull()
        {
            //Arrange
            CancellationToken token = default;

            //Act
            var actual = await _chatMessageRepository.GetAsync(Guid.NewGuid(), token);

            //Assert
            actual.Should().BeNull();
        }

        [Fact]
        public async void DeleteAsync_DeleteExistedMessageFromDb_DeletionStatusIsDeleted() 
        {
            //Arrange
            CancellationToken token = default;
            var sender = _chatUserFixtureHelper.CreateRandomChatUser();
            var userRecipient = _chatUserFixtureHelper.CreateRandomChatUser();
            var message = _chatMessageFixtureHelper.CreateRandomChatMessage(sender, userRecipient);
            var expected = DeletionStatus.Deleted;

            await _chatUserRepository.CreateAsync(sender, token);
            await _chatUserRepository.CreateAsync(userRecipient, token);
            await _chatMessageRepository.CreateAsync(message, token);

            //Act
            var actual = await _chatMessageRepository.DeleteAsync(message.ID, token);
            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public async void DeleteAsync_DeleteInexistentMessageFromDb_DeletionStatusIsNotExisted()
        {
            // Arrange
            CancellationToken token = default;
            DeletionStatus expectedStatus = DeletionStatus.NotExisted;
            // Act
            var actual = await _chatMessageRepository.DeleteAsync(Guid.NewGuid(), token);

            // Assert
            actual.Should().Be(expectedStatus);
        }

        [Fact]
        public async void UpdateAsync_UpdateExistedMessageInDb_MessageUpdated()
        {
            //Arrange
            CancellationToken token = default;
            var sender = _chatUserFixtureHelper.CreateRandomChatUser();
            var userRecipient = _chatUserFixtureHelper.CreateRandomChatUser();
            var message = _chatMessageFixtureHelper.CreateRandomChatMessage(sender, userRecipient);

            await _chatUserRepository.CreateAsync(sender, token);
            await _chatUserRepository.CreateAsync(userRecipient, token);
            await _chatMessageRepository.CreateAsync(message, token);

            var updateModel = new UpdateChatMessageModel()
            {
                ID = message.ID,
                Body = "New"
            };

            var expected = new ChatMessageModel() 
            {
                ID = message.ID,
                Body = updateModel.Body,
                IsEdited = updateModel.IsEdited,
                Sent = message.Sent,
                IsRead = message.IsRead,
                Sender = message.Sender,
                RecipientUser = message.RecipientUser,
                RecipientGroup = message.RecipientGroup
            };

            //Act
            await _chatMessageRepository.UpdateAsync(updateModel, token);
            var actual = await _chatMessageRepository.GetAsync(expected.ID, token);
            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void UpdateAsync_UpdateUnxistentMessageInDb_ReturnFalse()
        {
            //Arrange
            CancellationToken token = default;
            var updateModel = new UpdateChatMessageModel()
            {
                ID = Guid.NewGuid(),
                Body = "New"
            };

            //Act
            var actual = await _chatMessageRepository.UpdateAsync(updateModel, token);
            await _databaseFixture.ClearDatabaseAsync();
            //Assert
            Assert.False(actual);
        }

        [Fact]
        public async void ListAsync_GetMesageFromDefinedSenderAndRecipient_ReturnsAllMessagesBetweenSenderAndUserRecipient() 
        {
            //Arrange
            CancellationToken token = default;
            var sender = _chatUserFixtureHelper.CreateRandomChatUser();
            var userRecipient = _chatUserFixtureHelper.CreateRandomChatUser();
            var otherUser = _chatUserFixtureHelper.CreateRandomChatUser();
            var messages = _chatMessageFixtureHelper.CreateRandomChatMessagesList(5,sender, userRecipient);
            var otherMessage = _chatMessageFixtureHelper.CreateRandomChatMessage(sender, otherUser);
            var listParameters = new ChatMessageListParameters()
            {
                SortOrder = SortOrder.Asc,
                SortBy = ChatMessageSort.Sent,
                RecipientIsGroup = false,
                Sender = sender.ID,
                Recipient = userRecipient.ID,
            };

            await _chatUserRepository.CreateAsync(sender, token);
            await _chatUserRepository.CreateAsync(userRecipient, token);
            await _chatUserRepository.CreateAsync(otherUser, token);
            await _chatMessageRepository.CreateAsync(otherMessage, token);
            foreach (var message in messages) 
            {
                await _chatMessageRepository.CreateAsync(message, token);
            }
            var expected = messages.OrderBy(x => x.Sent).ToList();

            //Act
            var actual = await _chatMessageRepository.ListAsync(listParameters,token);
            await _databaseFixture.ClearDatabaseAsync();
            //Assert
            actual.Should().BeEquivalentTo(expected);

        }

        [Fact]
        public async void ListAsync_GetMesageFromDefinedSenderAndRecipient_ReturnsAllMessagesBetweenSenderAndGroupRecipient()
        {
            //Arrange
            CancellationToken token = default;
            var sender = _chatUserFixtureHelper.CreateRandomChatUser();
            var groupRecipient = _groupChatFixtureHelper.CreateRandomGroupChat();
            var otherUser = _chatUserFixtureHelper.CreateRandomChatUser();
            var messages = _chatMessageFixtureHelper.CreateRandomChatMessagesList(5, sender, groupRecipient);
            var otherMessage = _chatMessageFixtureHelper.CreateRandomChatMessage(sender, otherUser);
            var listParameters = new ChatMessageListParameters()
            {
                SortOrder = SortOrder.Asc,
                SortBy = ChatMessageSort.Sent,
                RecipientIsGroup = true,
                Sender = sender.ID,
                Recipient = groupRecipient.ID,
            };

            await _chatUserRepository.CreateAsync(sender, token);
            await _groupChatRepository.CreateGroupChatAsync(groupRecipient, token);
            await _chatUserRepository.CreateAsync(otherUser, token);
            await _chatMessageRepository.CreateAsync(otherMessage, token);
            foreach (var message in messages)
            {
                await _chatMessageRepository.CreateAsync(message, token);
            }
            var expected = messages.OrderBy(x => x.Sent).ToList();

            //Act
            var actual = await _chatMessageRepository.ListAsync(listParameters, token);
            await _databaseFixture.ClearDatabaseAsync();
            //Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void ListAsync_SendNullListParameters_ThrowsArgumentNullException()
        {
            //Arrange
            CancellationToken token = default;
            ChatMessageListParameters listParameters = null;

            //Assert
            Assert.ThrowsAsync<ArgumentNullException>(
                () => _chatMessageRepository.ListAsync(listParameters, token));
        }
    }
}
