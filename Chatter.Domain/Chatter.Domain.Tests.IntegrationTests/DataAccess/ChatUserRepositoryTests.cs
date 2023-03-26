using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.DbOptions;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.DataAccess.Models.Pagination;
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
    public class ChatUserRepositoryTests
    {
        private readonly DatabaseFixture _databaseFixture;

        private readonly ChatUserRepository _chatUserRepository;

        private readonly ChatUserFixtureHelper _chatUserFixtureHelper;

        public ChatUserRepositoryTests()
        {
            _databaseFixture = new DatabaseFixture();
            _databaseFixture.EnsureCreated(true);

            _chatUserFixtureHelper = new ChatUserFixtureHelper();

            var chatUserRepoLoggerMock = new Mock<ILogger<ChatUserRepository>>();

            var optionsMock = new Mock<IOptions<DatabaseOptions>>();
            optionsMock.Setup(x => x.Value)
            .Returns(_databaseFixture.dbOptions);

            _chatUserRepository = new ChatUserRepository(optionsMock.Object, chatUserRepoLoggerMock.Object);
        }

        [Fact]
        public async void GetAsync_GetExitedChatUserFromDb_ReturnsExpectedChatUser() 
        {
            //Assert
            CancellationToken token = default;
            var expected = _chatUserFixtureHelper.CreateRandomChatUser(); 
            await _chatUserRepository.CreateAsync(expected,token);

            //Act
            var actual = await _chatUserRepository.GetAsync(expected.ID, token);
            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(expected);

        }

        [Fact]
        public async void GetAsync_GetInexistentChatUserInDb_ReturnsNull()
        {
            //Arrange
            CancellationToken token = default;

            //Act
            var actual = await _chatUserRepository.GetAsync(Guid.NewGuid(), token);

            //Assert
            actual.Should().BeNull();
        }

        [Fact]
        public async void DeleteAsync_DeleteExistedChatUserFromDb_DeletionStatusIsDeleted() 
        {
            //Arrange
            CancellationToken token = default;
            var user = _chatUserFixtureHelper.CreateRandomChatUser();
            var expected = DeletionStatus.Deleted;

            await _chatUserRepository.CreateAsync(user, token);

            //Act
            var actual = await _chatUserRepository.DeleteAsync(user.ID, token);
            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public async void DeleteAsync_DeleteInexistentChatUserFromDb_DeletionStatusIsNotExisted() 
        {
            // Arrange
            CancellationToken token = default;
            DeletionStatus expectedStatus = DeletionStatus.NotExisted;
            // Act
            var actual = await _chatUserRepository.DeleteAsync(Guid.NewGuid(), token);

            // Assert
            actual.Should().Be(expectedStatus);
        }

        [Fact]
        public async void UpdateAsync_UpdateExistedChatUserInDb_UserUpdated() 
        {
            //Arrange
            CancellationToken token = default;
            var user = _chatUserFixtureHelper.CreateRandomChatUser();

            await _chatUserRepository.CreateAsync(user, token);
            var formatedLastActive = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.ff");
            var formatedBlockedUntil = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.ff");
            var updateModel = new UpdateChatUserModel() 
            {
                ID = user.ID,
                LastName = "New",
                FirstName = "New",
                Patronymic = "New",
                UniversityName = "New",
                UniversityFaculty = "New",
                LastActive = DateTime.Parse(formatedLastActive),
                IsBlocked = !user.IsBlocked,
                BlockedUntil = DateTime.Parse(formatedBlockedUntil)
            };

            var expected = new ChatUserModel() 
            {
                ID = updateModel.ID,
                LastName = updateModel.LastName,
                FirstName = updateModel.FirstName,
                Patronymic = updateModel.Patronymic,
                UniversityName = updateModel.UniversityName,
                UniversityFaculty = updateModel.UniversityFaculty,
                JoinedUtc = user.JoinedUtc,
                LastActive = (DateTime)updateModel.LastActive,
                IsBlocked = (bool)updateModel.IsBlocked,
                BlockedUntil = (DateTime)updateModel.BlockedUntil
            };


            //Act
            await _chatUserRepository.UpdateAsync(updateModel, token);
            var actual = await _chatUserRepository.GetAsync(expected.ID, token);
            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actual.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async void UpdateAsync_UpdateUnxistentChatUserInDb_ReturnFalse()
        {
            //Arrange
            CancellationToken token = default;
            var updateModel = new UpdateChatUserModel()
            {
                ID = Guid.NewGuid(),
                LastName = "New",
                FirstName = "New",
                Patronymic = "New",
            };

            //Act
            var actual = await _chatUserRepository.UpdateAsync(updateModel, token);
            await _databaseFixture.ClearDatabaseAsync();
            //Assert
            Assert.False(actual);
        }


        [Fact]
        public async void ListAsync_GetFirstPage_ResultMatchesWithExpected() 
        {
            //Arrange
            CancellationToken token = default;
            var totalUsersCount = 5;
            var totalPagesCount = 2;
            var pageNumber = 1;
            var pageSize = 3;
            var users = _chatUserFixtureHelper.CreateRandomUsersList(totalUsersCount);
            foreach (var user in users) 
            {
                await _chatUserRepository.CreateAsync(user, token);
            }
            var listParameters = new ChatUserListParameters() 
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortOrder = SortOrder.Asc,
                SortBy = ChatUserSort.LastName,
            };
            var expectedPaginatedResult = new PaginatedResult<ChatUserModel, ChatUserSort>()
            {
                TotalCount = totalUsersCount,
                TotalPages = totalPagesCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortOrder = listParameters.SortOrder,
                SortBy = listParameters.SortBy,
            };
            var expectedResultEntities = users
                .OrderBy(x => x.LastName)
                .Take(pageSize)
                .ToList();

            //Act
            var actualPaginatedResult = await _chatUserRepository.ListAsync(listParameters,token);

            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actualPaginatedResult.Should().NotBeNull();
            actualPaginatedResult.Should().BeEquivalentTo(expectedPaginatedResult, o => o.Excluding(x => x.Entities));
            actualPaginatedResult.Entities.Should().NotBeNull();
            actualPaginatedResult.Entities.Should().BeEquivalentTo(expectedResultEntities);
        }

        [Fact]
        public async void ListAsync_GetSecondPage_ResultMatchesWithExpected()
        {
            //Arrange
            CancellationToken token = default;
            var totalUsersCount = 5;
            var totalPagesCount = 2;
            var pageNumber = 2;
            var pageSize = 3;
            var expectedCountOnLastPage = 2;
            var users = _chatUserFixtureHelper.CreateRandomUsersList(totalUsersCount);
            foreach (var user in users)
            {
                await _chatUserRepository.CreateAsync(user, token);
            }
            var listParameters = new ChatUserListParameters()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortOrder = SortOrder.Desc,
                SortBy = ChatUserSort.FirstName,
            };
            var expectedPaginatedResult = new PaginatedResult<ChatUserModel, ChatUserSort>()
            {
                TotalCount = totalUsersCount,
                TotalPages = totalPagesCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortOrder = listParameters.SortOrder,
                SortBy = listParameters.SortBy,
            };
            var expectedResultEntities = users
                .OrderByDescending(x => x.FirstName)
                .TakeLast(expectedCountOnLastPage)
                .ToList();

            //Act
            var actualPaginatedResult = await _chatUserRepository.ListAsync(listParameters, token);

            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actualPaginatedResult.Should().NotBeNull();
            actualPaginatedResult.Should().BeEquivalentTo(expectedPaginatedResult, o => o.Excluding(x => x.Entities));
            actualPaginatedResult.Entities.Should().NotBeNull();
            actualPaginatedResult.Entities.Should().BeEquivalentTo(expectedResultEntities);
        }

        [Fact]
        public async void ListAsync_GetUsersWithSpecificUniversityName_ResultMatchesWithExpected()
        {
            //Arrange
            CancellationToken token = default;
            var univerityName = "KhNUE";
            var totalUsersCount = 5;
            
            var totalPagesCount = 1;
            var pageNumber = 1;
            var pageSize = 3;
            var users = _chatUserFixtureHelper.CreateRandomUsersList(totalUsersCount);

            for (int i = 0; i < 2; i++) 
            {
                users[i].UniversityName = univerityName;
            }

            foreach (var user in users)
            {
                await _chatUserRepository.CreateAsync(user, token);
            }
            var listParameters = new ChatUserListParameters()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortOrder = SortOrder.Asc,
                SortBy = ChatUserSort.Patronymic,
                UniversitiesNames = new List<string>() { univerityName }
            };
            var expectedResultEntities = users
               .OrderBy(x => x.Patronymic)
               .Where(x => x.UniversityName == univerityName)
               .ToList();
            var totalUsersWithSameUniversityName = expectedResultEntities.Count;
            var expectedPaginatedResult = new PaginatedResult<ChatUserModel, ChatUserSort>()
            {
                TotalCount = totalUsersWithSameUniversityName,
                TotalPages = totalPagesCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortOrder = listParameters.SortOrder,
                SortBy = listParameters.SortBy,
            };
           

            //Act
            var actualPaginatedResult = await _chatUserRepository.ListAsync(listParameters, token);

            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actualPaginatedResult.Should().NotBeNull();
            actualPaginatedResult.Should().BeEquivalentTo(expectedPaginatedResult, o => o.Excluding(x => x.Entities));
            actualPaginatedResult.Entities.Should().NotBeNull();
            actualPaginatedResult.Entities.Should().BeEquivalentTo(expectedResultEntities);
        }
        [Fact]
        public async void ListAsync_GetUsersWithSpecificUniversityNameAndFaculty_ResultMatchesWithExpected()
        {
            //Arrange
            CancellationToken token = default;
            var univerityName = "KhNUE";
            var univerityFaculty = "IT";
            var totalUsersCount = 5;
            var totalPagesCount = 1;
            var pageNumber = 1;
            var pageSize = 3;
            var users = _chatUserFixtureHelper.CreateRandomUsersList(totalUsersCount);

            for (int i = 0; i < 2; i++)
            {
                users[i].UniversityName = univerityName;
            }
            for (int i = 0; i < 2; i++)
            {
                users[i].UniversityFaculty = univerityFaculty;
            }
            foreach (var user in users)
            {
                await _chatUserRepository.CreateAsync(user, token);
            }
            var listParameters = new ChatUserListParameters()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortOrder = SortOrder.Asc,
                SortBy = ChatUserSort.Patronymic,
                UniversitiesNames = new List<string>() { univerityName, univerityFaculty }
            };
            var expectedResultEntities = users
               .OrderBy(x => x.Patronymic)
               .Where(x => x.UniversityName == univerityName && x.UniversityFaculty == univerityFaculty)
               .ToList();
            var totalUsersWithSameUniversityName = expectedResultEntities.Count;
            var expectedPaginatedResult = new PaginatedResult<ChatUserModel, ChatUserSort>()
            {
                TotalCount = totalUsersWithSameUniversityName,
                TotalPages = totalPagesCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortOrder = listParameters.SortOrder,
                SortBy = listParameters.SortBy,
            };


            //Act
            var actualPaginatedResult = await _chatUserRepository.ListAsync(listParameters, token);

            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actualPaginatedResult.Should().NotBeNull();
            actualPaginatedResult.Should().BeEquivalentTo(expectedPaginatedResult, o => o.Excluding(x => x.Entities));
            actualPaginatedResult.Entities.Should().NotBeNull();
            actualPaginatedResult.Entities.Should().BeEquivalentTo(expectedResultEntities);
        }

        [Fact]
        public async void ListAsync_SendNullListParameters_ThrowsArgumentNullException()
        {
            //Arrange
            CancellationToken token = default;
            ChatUserListParameters listParameters = null;

            //Assert
            Assert.ThrowsAsync<ArgumentNullException>(
                () => _chatUserRepository.ListAsync(listParameters, token));
        }

        [Fact]
        public async void ListAsync_GetAllChatUsersListFromEmptyDb_PaginatedResultEntitiesAreNull()
        {
            CancellationToken token = default;
            var listParameters = new ChatUserListParameters()
            {
                PageNumber = 1,
                PageSize = 10,
                SortOrder = SortOrder.Asc,
                SortBy = ChatUserSort.LastName,
            };

            var expectedPaginatedResult = new PaginatedResult<ChatUserModel, ChatUserSort>()
            {
                TotalCount = 0,
                TotalPages = 0,
                PageNumber = 1,
                PageSize = 10,
                SortOrder = SortOrder.Asc,
                SortBy = ChatUserSort.LastName,
                Entities = new List<ChatUserModel>(),
            };

            //Act
            var actualPaginatedResult = await _chatUserRepository.ListAsync(listParameters, token);

            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actualPaginatedResult.Should().BeEquivalentTo(expectedPaginatedResult, o => o.Excluding(x => x.Entities));
            actualPaginatedResult.Entities.Count.Should().Be(0);
        }
    }
}
