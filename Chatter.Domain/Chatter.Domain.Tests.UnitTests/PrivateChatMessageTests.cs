using AutoFixture;
using Chatter.Domain.BusinessLogic.Models;
using Chatter.Domain.BusinessLogic.Services;
using Chatter.Domain.BusinessLogic.Enums;
using Chatter.Domain.DataAccess.Interfaces;
using Chatter.Domain.Common.Enums;
using Chatter.Domain.Tests.Common;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.BusinessLogic.Models.Update;
using Chatter.Domain.DataAccess.Models.Parameters;
using Chatter.Domain.BusinessLogic.Models.Chats;
using AutoFixture.Kernel;
using Chatter.Domain.BusinessLogic.Models.Abstract;
using Chatter.Domain.BusinessLogic.Models.Create;

namespace Chatter.Domain.Tests.UnitTests
{
    public class PrivateChatMessageTests
    {
        private readonly PrivateChatService _privateChatService;
        private readonly Mock<IChatUserRepository> _chatUserRepositoryMock;
        private readonly Mock<IChatMessageRepository> _chatMessageRepositoryMock;

        private readonly ChatMessageFixtureHelper _chatMessageFixtureHelper;
        private readonly ChatUserFixtureHelper _chatUserFixtureHelper;
        private readonly Fixture _fixture;

        public PrivateChatMessageTests()
        {
            var loggerMock = new Mock<ILogger<PrivateChatService>>();
            _chatUserRepositoryMock = new Mock<IChatUserRepository>();
            _chatMessageRepositoryMock = new Mock<IChatMessageRepository>();

            _privateChatService = new PrivateChatService(_chatUserRepositoryMock.Object, _chatMessageRepositoryMock.Object, loggerMock.Object);

            _fixture = new Fixture();
            _fixture.Customizations.Add(new TypeRelay(typeof(User),typeof(ChatUser)));

            _chatMessageFixtureHelper = new ChatMessageFixtureHelper();
            _chatUserFixtureHelper = new ChatUserFixtureHelper();
        }

        [Fact]
        public async void CreateNewChat_CreateNewChatWithExistedMembers_ReturnsServiceResultWithChatModel() 
        {
            //Arrange
            CancellationToken token = default;
            var resultFromDb = _chatUserFixtureHelper.CreateRandomChatUser();
            
            var member = _fixture.Create<ChatUser>();
            member.ID = resultFromDb.ID;

            _chatUserRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), token))
                .Returns(Task.FromResult(resultFromDb));

            //Act
            var actual = await _privateChatService.CreateNewChat(member, member, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.IsEmpty.Should().BeFalse();
            actual.Value.Member1.Should().BeEquivalentTo(member);
            actual.Value.Member2.Should().BeEquivalentTo(member);
        }

        [Fact]
        public async void CreateNewChat_CreateNewChatWithInexistentMembers_ReturnsServiceResultWithBusinessError()
        {
            //Arrange
            CancellationToken token = default;
            var member = _fixture.Create<ChatUser>();

            //Act
            var actual = await _privateChatService.CreateNewChat(member, member, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(ErrorType.BusinessError);
        }

        [Fact]
        public async void DeleteMessage_DeleteExistedMessage_ReturnsServiceResultWithDeletedMessageId()
        {
            //Arrange
            CancellationToken token = default;
            var messageId = Guid.NewGuid();
            _chatMessageRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<Guid>(), token))
               .Returns(Task.FromResult(DeletionStatus.Deleted));

            //Act
            var actual = await _privateChatService.DeleteMessage(messageId, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.IsEmpty.Should().BeFalse();
            actual.Value.Should().Be(messageId);
        }

        [Fact]
        public async void DeleteMessage_DeleteInexistentMessage_ReturnsServiceResultWithBusinessError()
        {
            //Arrange
            CancellationToken token = default;

            //Act
            var actual = await _privateChatService.DeleteMessage(Guid.NewGuid(), token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(ErrorType.BusinessError);
        }

        [Fact]
        public async void EditMessage_EditExistedMessage_ReturnsServiceResultWithMessageId()
        {
            //Arrange
            CancellationToken token = default;
            var updateModel = new UpdateMessage() 
            {
                ID = Guid.NewGuid(),
                Body = _fixture.Create<string>(),
            };

            _chatMessageRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<UpdateChatMessageModel>(), token))
               .Returns(Task.FromResult(true));


            //Act
            var actual = await _privateChatService.EditMessage(updateModel, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Should().Be(updateModel.ID);
        }

        [Fact]
        public async void EditMessage_EditInexistentMessage_ReturnsServiceResultWithBusinessError()
        {
            //Arrange
            CancellationToken token = default;
            var updateModel = new UpdateMessage()
            {
                ID = Guid.NewGuid(),
                Body = _fixture.Create<string>(),
            };

            //Act
            var actual = await _privateChatService.EditMessage(updateModel, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(ErrorType.BusinessError);
        }

        [Fact]
        public async void LoadChat_GetMessagesListBetweenMember1AndMember2_ReturnsServiceResultWithMessagesList()
        {
            //Arrange
            CancellationToken token = default;
            var chat = _fixture.Create<PrivateChat>();
            var listFromDb = _fixture.Create<IList<ChatMessageModel>>();
            
            _chatMessageRepositoryMock.Setup(x => x.ListAsync(It.IsAny<ChatMessageListParameters>(), token))
                .Returns(Task.FromResult(listFromDb));

            //Act
            var actual = await _privateChatService.LoadChat(chat, token);
            
            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Should().NotBeNull();
            Assert.True(actual.Value.All(x => x.Recipient.ID == chat.Member1.ID || x.Sender.ID == chat.Member2.ID));
        }

        [Fact]
        public async void LoadChat_GetEmptyMessagesListBetweenMember1AndMember2_ReturnsServiceResultWithEmptyMessagesList()
        {
            //Arrange
            CancellationToken token = default;
            var chat = _fixture.Create<PrivateChat>();
            IList<ChatMessageModel> listFromDb = new List<ChatMessageModel>();

            _chatMessageRepositoryMock.Setup(x => x.ListAsync(It.IsAny<ChatMessageListParameters>(), token))
                .Returns(Task.FromResult(listFromDb));

            //Act
            var actual = await _privateChatService.LoadChat(chat, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Should().NotBeNull();
            actual.Value.Count.Should().Be(0);
        }

        [Fact]
        public async void MarkMessageAsRead_UpdateExistedMessageReadStatus_ReturnsServiceResultWithMessageID() 
        {
            //Arrange
            CancellationToken token = default;
            _chatMessageRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<UpdateChatMessageModel>(), token))
                .Returns(Task.FromResult(true));
            var expected = Guid.NewGuid();

            //Act
            var actual = await _privateChatService.MarkMessageAsRead(expected, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Should().Be(expected);
        }

        [Fact]
        public async void MarkMessageAsRead_UpdateInexistentMessage_ReturnsServiceResultWithBusinessError()
        {
            //Arrange
            CancellationToken token = default;
            var expected = Guid.NewGuid();

            //Act
            var actual = await _privateChatService.MarkMessageAsRead(expected, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(ErrorType.BusinessError);
        }


        [Fact]
        public async void SendMessageAsync_SendMessageWithExistedSenderAndRecipient_ReturnsServiceResultWithNewMessageID()
        {
            //Arrange
            CancellationToken token = default;
            var modelFromDb = _fixture.Create<ChatUserModel>();
            _chatUserRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), token))
                .Returns(Task.FromResult(modelFromDb));
            var createModel = _fixture.Create<CreateChatMessage>();
            
            //Act
            var actual = await _privateChatService.SendMessageAsync(createModel, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async void SendMessageAsync_SendMessageWithInexistentSenderAndRecipient_ReturnsServiceResultWithBusinessError()
        {
            //Arrange
            CancellationToken token = default;
            var createModel = _fixture.Create<CreateChatMessage>();

            //Act
            var actual = await _privateChatService.SendMessageAsync(createModel, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Value.Should().Be(Guid.Empty);
            actual.Error.Type.Should().Be(ErrorType.BusinessError);

        }
    }
}
