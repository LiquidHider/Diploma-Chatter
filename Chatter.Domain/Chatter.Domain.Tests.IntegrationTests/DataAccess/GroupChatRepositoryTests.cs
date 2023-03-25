using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.DbOptions;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.DataAccess.Models.Parameters;
using Chatter.Domain.DataAccess.Repositories;
using Chatter.Domain.IntegrationTests.Database.DatabaseFixture;
using Chatter.Domain.Tests.IntegrationTests.DataAccess.FixturesHelpers;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;

namespace Chatter.Domain.Tests.IntegrationTests.DataAccess
{
    public class GroupChatRepositoryTests
    {
        private readonly GroupChatRepository _groupChatRepository;
        private readonly ChatUserRepository _chatUserRepository;
        private readonly DatabaseFixture _databaseFixture;
        private readonly GroupChatFixtureHelper _groupChatFixtureHelper;
        private readonly ChatUserFixtureHelper _chatUserFixtureHelper;


        public GroupChatRepositoryTests()
        {
            _databaseFixture = new DatabaseFixture();
            _databaseFixture.EnsureCreated(false);
            _groupChatFixtureHelper = new GroupChatFixtureHelper();
            _chatUserFixtureHelper = new ChatUserFixtureHelper();
            var groupCharRepoLoggerMock = new Mock<ILogger<GroupChatRepository>>();
            var chatUserRepoLoggerMock = new Mock<ILogger<ChatUserRepository>>();

            var optionsMock = new Mock<IOptions<DatabaseOptions>>();
            optionsMock.Setup(x => x.Value)
            .Returns(_databaseFixture.dbOptions);

            _chatUserRepository = new ChatUserRepository(optionsMock.Object, chatUserRepoLoggerMock.Object);
            _groupChatRepository = new GroupChatRepository(optionsMock.Object, groupCharRepoLoggerMock.Object);
        }

        [Fact]
        public async void GetGroupChatAsync_GetExistedGroupChatInDb_ReturnsGroupChatModel() 
        {
            //Arrange
            CancellationToken token = default;
            var expectedModel = _groupChatFixtureHelper.CreateRandomGroupChat();
            await _groupChatRepository.CreateGroupChatAsync(expectedModel, token);

            //Act
            var actual = await _groupChatRepository.GetGroupChatAsync(expectedModel.ID,token);
            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(expectedModel);

        }

        [Fact]
        public async void GetGroupChatAsync_GetInexistentPhotoInDb_ThrowsInvalidOperationException()
        {
            //Arrange
            CancellationToken token = default;

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                 () => _groupChatRepository.GetGroupChatAsync(Guid.NewGuid(), token));
        }

        [Fact]
        public async void DeleteGroupChatAsync_DeleteExistedGroupFromDb_DeletionStatusIsDeleted() 
        {
            //Arrange
            CancellationToken token = default;
            DeletionStatus expected = DeletionStatus.Deleted;
            var modelToDelete = _groupChatFixtureHelper.CreateRandomGroupChat();
            await _groupChatRepository.CreateGroupChatAsync(modelToDelete, token);

            //Act
            var actual = await _groupChatRepository.DeleteGroupChatAsync(modelToDelete.ID, token);
            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public async void DeleteGroupChatAsync_DeleteInexistentGroupChatFromDb_DeletionStatusIsNotExisted()
        {
            //Arrange
            CancellationToken token = default;
            DeletionStatus expectedStatus = DeletionStatus.NotExisted;

            //Act
            var actual = await _groupChatRepository.DeleteGroupChatAsync(Guid.NewGuid(), token);

            await _databaseFixture.ClearDatabaseAsync();
            //Assert
            actual.Should().Be(expectedStatus);
        }

        [Fact]
        public async void DeleteGroupChatAsync_DeleteExistedGroupWithDeclaredParticipantsFromDb_UserJoinedGroupsTableIsEmpty() 
        {
            //Arrange
            CancellationToken token = default;
            var group = _groupChatFixtureHelper.CreateRandomGroupChat();
            var user = _chatUserFixtureHelper.CreateRandomChatUser();
            var command = new CommandDefinition("SELECT COUNT(*) FROM [dbo].[UserJoinedGroups]");
            var groupParticipant = new GroupUserModel() 
            {
                ID = Guid.NewGuid(),
                GroupID = group.ID,
                UserID = user.ID,
                UserRole = GroupUserRole.Subscriber,
            };
            await _chatUserRepository.CreateAsync(user,token);
            await _groupChatRepository.CreateGroupChatAsync(group, token);
            await _groupChatRepository.AddGroupParticipantAsync(groupParticipant, token);

            //Act
            await _groupChatRepository.DeleteGroupChatAsync(group.ID, token);
            var queryResult = await _databaseFixture.QueryAsync<int>(command);
            var actual = queryResult.First();
            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            Assert.True(actual == 0);
        }

        [Fact]
        public async void DeleteGroupChatAsync_DeleteExistedGroupWithDeclaredParticipantsFromDb_BlockedGroupChatUsersTableIsEmpty()
        {
            //Arrange
            CancellationToken token = default;
            var group = _groupChatFixtureHelper.CreateRandomGroupChat();
            var user = _chatUserFixtureHelper.CreateRandomChatUser();
            var command = new CommandDefinition("SELECT COUNT(*) FROM [dbo].[BlockedGroupChatUsers]");
            var groupParticipant = new GroupUserModel()
            {
                ID = Guid.NewGuid(),
                GroupID = group.ID,
                UserID = user.ID,
                UserRole = GroupUserRole.Subscriber,
            };
            var blockModel = new GroupChatBlockModel() 
            {
                ID = Guid.NewGuid(),
                UserID = user.ID,
                GroupID = group.ID,
                BlockedUntil = DateTime.UtcNow,
            };
            await _chatUserRepository.CreateAsync(user, token);
            await _groupChatRepository.CreateGroupChatAsync(group, token);
            await _groupChatRepository.AddGroupParticipantAsync(groupParticipant, token);
            await _groupChatRepository.SetGroupUserAsBlockedAsync(blockModel, token);
           
            //Act
            await _groupChatRepository.DeleteGroupChatAsync(group.ID, token);
            var queryResult = await _databaseFixture.QueryAsync<int>(command);
            var actual = queryResult.First();
            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            Assert.True(actual == 0);
        }

        [Fact]
        public async void DeleteGroupUserFromBlockedUsersAsync_DeleteExistedGroupFromDb_DeletionStatusIsDeleted()
        {
            //Arrange
            CancellationToken token = default;
            var group = _groupChatFixtureHelper.CreateRandomGroupChat();
            var user = _chatUserFixtureHelper.CreateRandomChatUser();
            var expected = DeletionStatus.Deleted;
            var groupParticipant = new GroupUserModel()
            {
                ID = Guid.NewGuid(),
                GroupID = group.ID,
                UserID = user.ID,
                UserRole = GroupUserRole.Subscriber,
            };
            var blockModel = new GroupChatBlockModel()
            {
                ID = Guid.NewGuid(),
                UserID = user.ID,
                GroupID = group.ID,
                BlockedUntil = DateTime.UtcNow,
            };
            await _chatUserRepository.CreateAsync(user, token);
            await _groupChatRepository.CreateGroupChatAsync(group, token);
            await _groupChatRepository.AddGroupParticipantAsync(groupParticipant, token);
            await _groupChatRepository.SetGroupUserAsBlockedAsync(blockModel, token);

            //Act
            var actual = await _groupChatRepository.DeleteGroupUserFromBlockedUsersAsync(blockModel.ID, token);
            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public async void DeleteGroupUserFromBlockedUsersAsync_DeleteInexistentChatUserFromDb_DeletionStatusIsNotExisted()
        {
            //Arrange
            CancellationToken token = default;
            DeletionStatus expectedStatus = DeletionStatus.NotExisted;

            //Act
            var actual = await _groupChatRepository.DeleteGroupUserFromBlockedUsersAsync(Guid.NewGuid(), token);

            await _databaseFixture.ClearDatabaseAsync();
            //Assert
            actual.Should().Be(expectedStatus);
        }

        [Fact]
        public async void DeleteGroupParticipantAsync_DeleteExistedGroupParticipantFromDb_DeletionStatusIsDeleted()
        {
            //Arrange
            CancellationToken token = default;
            var group = _groupChatFixtureHelper.CreateRandomGroupChat();
            var user = _chatUserFixtureHelper.CreateRandomChatUser();
            var expected = DeletionStatus.Deleted;
            var groupParticipant = new GroupUserModel()
            {
                ID = Guid.NewGuid(),
                GroupID = group.ID,
                UserID = user.ID,
                UserRole = GroupUserRole.Subscriber,
            };
            var blockModel = new GroupChatBlockModel()
            {
                ID = Guid.NewGuid(),
                UserID = user.ID,
                GroupID = group.ID,
                BlockedUntil = DateTime.UtcNow,
            };
            await _chatUserRepository.CreateAsync(user, token);
            await _groupChatRepository.CreateGroupChatAsync(group, token);
            await _groupChatRepository.AddGroupParticipantAsync(groupParticipant, token);

            //Act
            var actual = await _groupChatRepository.DeleteGroupParticipantAsync(groupParticipant.ID, token);
            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public async void DeleteGroupUserFromBlockedUsersAsync_DeleteInexistentPhotoFromDb_DeletionStatusIsNotExisted()
        {
            //Arrange
            CancellationToken token = default;
            DeletionStatus expectedStatus = DeletionStatus.NotExisted;

            //Act
            var actual = await _groupChatRepository.DeleteGroupParticipantAsync(Guid.NewGuid(), token);

            await _databaseFixture.ClearDatabaseAsync();
            //Assert
            actual.Should().Be(expectedStatus);
        }

        [Fact]
        public async void UpdateAsync_UpdateExistedGroupChatInDb_GroupChatUpdated() 
        {
            //Arrange
            CancellationToken token = default;
            var group = _groupChatFixtureHelper.CreateRandomGroupChat();
            var updateGroup = new UpdateGroupChatModel()
            {
                ID = group.ID,
                Name = "New",
                Description = "NewDesc"

            };
            var expected = new GroupChatModel()
            {
                ID = group.ID,
                Name = "New",
                Description = "NewDesc"
            };
            await _groupChatRepository.CreateGroupChatAsync(group, token);

            //Act
            await _groupChatRepository.UpdateAsync(updateGroup, token);
            var actual = await _groupChatRepository.GetGroupChatAsync(group.ID, token);
            await _databaseFixture.ClearDatabaseAsync();
            //Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void UpdateAsync_UpdateUnxistentGroupChatInDb_ReturnIsFalse()
        {
            //Arrange
            CancellationToken token = default;
            var updateGroup = new UpdateGroupChatModel()
            {
                ID = Guid.NewGuid(),
                Name = "New",
                Description = "NewDesc"
            };

            //Act
            var actual = await _groupChatRepository.UpdateAsync(updateGroup, token);
            //Assert
            Assert.False(actual);
        }

        [Fact]
        public async void ListGroupChatsAsync_GetAllExistedChatGroupsSortByNameSortOrderAsc_ReturnsAllGroupsList() 
        {
            //Arrange 
            CancellationToken token = default;
            var listParameters = new GroupChatListParameters()
            {
                SortOrder = SortOrder.Asc,
                SortBy = GroupChatSort.Name
            };
            var groups = _groupChatFixtureHelper.CreateRandomGroupChatsList(5);
            foreach (var group in groups) 
            {
                await _groupChatRepository.CreateGroupChatAsync(group,token);
            }
            var expected = groups.OrderBy(x => x.Name).ToList();
            //Act
            var actual = await _groupChatRepository.ListGroupChatsAsync(listParameters, token);
            await _databaseFixture.ClearDatabaseAsync();
            //Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void ListGroupChatsAsync_GetAllExistedChatGroupsSortByIDSortOrderDesc_ReturnsAllGroupsList()
        {
            //Arrange 
            CancellationToken token = default;
            var listParameters = new GroupChatListParameters()
            {
                SortOrder = SortOrder.Desc,
                SortBy = GroupChatSort.ID
            };
            var groups = _groupChatFixtureHelper.CreateRandomGroupChatsList(5);
            foreach (var group in groups)
            {
                await _groupChatRepository.CreateGroupChatAsync(group, token);
            }
            var expected = groups.OrderByDescending(x => new SqlGuid(x.ID)).ToList();
            //Act
            var actual = await _groupChatRepository.ListGroupChatsAsync(listParameters, token);
            await _databaseFixture.ClearDatabaseAsync();
            //Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void ListGroupChatsAsync_GetExistedGroupsOfSpecificParticipant_ReturnsGroupsWithSameUserIdField() 
        {
            //Arrange 
            CancellationToken token = default;
            var user1 = _chatUserFixtureHelper.CreateRandomChatUser();
            var user2 = _chatUserFixtureHelper.CreateRandomChatUser();
            var groups = _groupChatFixtureHelper.CreateRandomGroupChatsList(5);
            var groupParticipants = CreateGroupUserModels(groups, new List<ChatUserModel>() { user1, user2 });
            
            var selectedParticipant = groupParticipants.First(x => x.UserID == user1.ID);
            var listParameters = new GroupChatListParameters()
            {
                SortOrder = SortOrder.Asc,
                SortBy = GroupChatSort.ID,
                UserId = selectedParticipant.UserID,
            };
            await _chatUserRepository.CreateAsync(user1, token);
            await _chatUserRepository.CreateAsync(user2, token);
            foreach (var group in groups)
            {
                await _groupChatRepository.CreateGroupChatAsync(group, token);
            }
            foreach (var groupUser in groupParticipants)
            {
                await _groupChatRepository.AddGroupParticipantAsync(groupUser, token);
            }
            await _groupChatRepository.DeleteGroupParticipantAsync(groupParticipants.First().ID, token);
            var expected = groups.TakeLast(groups.Count - 1);

            //Act
            var actual = await _groupChatRepository.ListGroupChatsAsync(listParameters, token);
            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void ListGroupParticipantsAsync_GetAllParticipantsOfSpecificGroup_ReturnsNotBlockedParticipantsWithSameGroupIdField() 
        {
            //Arrange
            CancellationToken token = default;
            var users = _chatUserFixtureHelper.CreateRandomUsersList(5);
            var group1 = _groupChatFixtureHelper.CreateRandomGroupChat();
            var group2 = _groupChatFixtureHelper.CreateRandomGroupChat();
            var participants = CreateGroupUserModels(new List<GroupChatModel> { group1 }, users);

            var listParameters = new GroupParticipantsListParameters()
            {
                GroupID = group1.ID,
                SortOrder = SortOrder.Asc,
                SortBy = GroupParticipantSort.UserRole,
                ShowBlocked = false
            };

            var blockModel = new GroupChatBlockModel()
            {
                ID = Guid.NewGuid(),
                UserID = users.First().ID,
                GroupID = group1.ID,
                BlockedUntil = DateTime.UtcNow
            };

            foreach (var user in users) 
            {
                await _chatUserRepository.CreateAsync(user, token);
            }

            await _groupChatRepository.CreateGroupChatAsync(group1, token);
            await _groupChatRepository.CreateGroupChatAsync(group2, token);

            foreach (var groupUser in participants)
            {
                await _groupChatRepository.AddGroupParticipantAsync(groupUser, token);
            }
            await _groupChatRepository.AddGroupParticipantAsync(new GroupUserModel()
            {
                ID = Guid.NewGuid(),
                UserID = users[0].ID,
                GroupID = group2.ID,
                UserRole = GroupUserRole.Subscriber
            }, token);

            await _groupChatRepository.SetGroupUserAsBlockedAsync(blockModel, token);

            var expected = participants.TakeLast(participants.Count - 1);
            //Act
            var actual = await _groupChatRepository.ListGroupParticipantsAsync(listParameters, token);
            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void ListGroupParticipantsAsync_GetAllParticipantsOfSpecificGroup_ReturnsAllParticipantsWithSameGroupIdField()
        {
            //Arrange
            CancellationToken token = default;
            var users = _chatUserFixtureHelper.CreateRandomUsersList(5);
            var group1 = _groupChatFixtureHelper.CreateRandomGroupChat();
            var group2 = _groupChatFixtureHelper.CreateRandomGroupChat();
            var participants = CreateGroupUserModels(new List<GroupChatModel> { group1 }, users);

            var listParameters = new GroupParticipantsListParameters()
            {
                GroupID = group1.ID,
                SortOrder = SortOrder.Desc,
                SortBy = GroupParticipantSort.UserRole,
                ShowBlocked = true
            };

            var blockModel = new GroupChatBlockModel()
            {
                ID = Guid.NewGuid(),
                UserID = users.First().ID,
                GroupID = group1.ID,
                BlockedUntil = DateTime.UtcNow
            };

            foreach (var user in users)
            {
                await _chatUserRepository.CreateAsync(user, token);
            }

            await _groupChatRepository.CreateGroupChatAsync(group1, token);
            await _groupChatRepository.CreateGroupChatAsync(group2, token);

            foreach (var groupUser in participants)
            {
                await _groupChatRepository.AddGroupParticipantAsync(groupUser, token);
            }
            await _groupChatRepository.AddGroupParticipantAsync(new GroupUserModel()
            {
                ID = Guid.NewGuid(),
                UserID = users[0].ID,
                GroupID = group2.ID,
                UserRole = GroupUserRole.Subscriber
            }, token);

            await _groupChatRepository.SetGroupUserAsBlockedAsync(blockModel, token);

            var expected = participants.OrderByDescending(x => x.UserRole).ToList();
            //Act
            var actual = await _groupChatRepository.ListGroupParticipantsAsync(listParameters, token);
            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void ListBlockedUsers_GetAllBlockedUsersOfSpecificGroup_ReturnsBlockedUsersList() 
        {
            //Assert
            CancellationToken token = default;
            var users = _chatUserFixtureHelper.CreateRandomUsersList(5);
            var group = _groupChatFixtureHelper.CreateRandomGroupChat();
            var participants = CreateGroupUserModels(new List<GroupChatModel> { group }, users);
            var formatedTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.ff");
            var blockedParticipants = participants.Take(participants.Count - 2).Select(x =>
                new GroupChatBlockModel()
                {
                    ID = Guid.NewGuid(),
                    UserID = x.UserID,
                    GroupID = x.GroupID,
                    BlockedUntil = DateTime.Parse(formatedTime)
                }
            ).ToList();

            foreach (var user in users)
            {
                await _chatUserRepository.CreateAsync(user, token);
            }

            await _groupChatRepository.CreateGroupChatAsync(group, token);

            foreach (var blockedUser in blockedParticipants)
            {
                await _groupChatRepository.SetGroupUserAsBlockedAsync(blockedUser, token);
            }

            //Act
            var actual = await _groupChatRepository.ListBlockedUsers(group.ID, token);
            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actual.Should().BeEquivalentTo(blockedParticipants);

        }

        [Fact]
        public async void ListBlockedUsers_GetAllBlockedUsersButWithNoUnblockTimeOfSpecificGroup_ReturnsBlockedUsersList()
        {
            //Assert
            CancellationToken token = default;
            var users = _chatUserFixtureHelper.CreateRandomUsersList(5);
            var group = _groupChatFixtureHelper.CreateRandomGroupChat();
            var participants = CreateGroupUserModels(new List<GroupChatModel> { group }, users);
            var blockedParticipants = participants.Take(participants.Count - 2).Select(x =>
                new GroupChatBlockModel()
                {
                    ID = Guid.NewGuid(),
                    UserID = x.UserID,
                    GroupID = x.GroupID,
                }
            ).ToList();

            foreach (var user in users)
            {
                await _chatUserRepository.CreateAsync(user, token);
            }

            await _groupChatRepository.CreateGroupChatAsync(group, token);

            foreach (var blockedUser in blockedParticipants)
            {
                await _groupChatRepository.SetGroupUserAsBlockedAsync(blockedUser, token);
            }

            //Act
            var actual = await _groupChatRepository.ListBlockedUsers(group.ID, token);
            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actual.Should().BeEquivalentTo(blockedParticipants);

        }

        private List<GroupUserModel> CreateGroupUserModels(List<GroupChatModel> groups, List<ChatUserModel> participants) 
        {
            var groupsUsers = new List<GroupUserModel>();
            foreach (var group in groups) 
            {
                foreach (var participant in participants) 
                {
                    groupsUsers.Add(new GroupUserModel() {
                    ID = Guid.NewGuid(),
                    UserID = participant.ID,
                    GroupID = group.ID,
                    UserRole = GroupUserRole.Subscriber
                    });
                }
            }
            return groupsUsers;
        }
    }
}
