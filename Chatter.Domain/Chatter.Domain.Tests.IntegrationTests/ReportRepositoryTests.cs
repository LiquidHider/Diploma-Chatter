using AutoFixture;
using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.DbOptions;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.DataAccess.Models.Pagination;
using Chatter.Domain.DataAccess.Models.Parameters;
using Chatter.Domain.DataAccess.Repositories;
using Chatter.Domain.IntegrationTests.Database.DatabaseFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Data.SqlTypes;

namespace Chatter.Domain.IntegrationTests
{
    public class ReportRepositoryTests
    {
        private readonly ReportRepository _reportRepository;
        private readonly ChatUserRepository _chatUserRepository;
        private readonly Fixture _fixture;
        private readonly DatabaseFixture _databaseFixture;

        public ReportRepositoryTests()
        {
            _databaseFixture = new DatabaseFixture();
            _databaseFixture.EnsureCreated(false);
            var reportRepoLoggerMock = new Mock<ILogger<ReportRepository>>();
            var chatUserRepoLoggerMock = new Mock<ILogger<ChatUserRepository>>();

            var optionsMock = new Mock<IOptions<DatabaseOptions>>();
             optionsMock.Setup(x => x.Value)
             .Returns(_databaseFixture.dbOptions);
            
            _fixture = new Fixture();

            _reportRepository = new ReportRepository(optionsMock.Object, reportRepoLoggerMock.Object);
            _chatUserRepository = new ChatUserRepository(optionsMock.Object, chatUserRepoLoggerMock.Object);
        }

        [Fact]
        public async void GetAsync_GetExistedReportInDb_ReturnsReportModel()
        {
            //Arrange
            CancellationToken token = default;
            ChatUserModel reportedChatUserModel = CreateRandomChatUser();
            await _chatUserRepository.CreateAsync(reportedChatUserModel, token);
            ReportModel reportModel = new ReportModel()
            {
                ID = Guid.NewGuid(),
                ReportedUserID = reportedChatUserModel.ID,
                Title = "Title",
                Message = "Message"
            };
            await _reportRepository.CreateAsync(reportModel, token); //add report which will be found

            //Act
            var actual = await _reportRepository.GetAsync(reportModel.ID, token);
            _databaseFixture.ClearDatabase();
            //Assert
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(reportModel); 
        }

        [Fact]
        public async void GetAsync_GetInexistentReportInDb_ThrowsInvalidOperationExeption()
        {
            //Arrange
            CancellationToken token = default;

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                 () => _reportRepository.GetAsync(Guid.NewGuid(),token));
        }

        [Fact]
        public async void DeleteAsync_DeleteExistedReportModelFromDb_DeletionStatusIsDeleted()
        {
            // Arrange
            CancellationToken token = default;
            ChatUserModel reportedChatUser = CreateRandomChatUser();
            await _chatUserRepository.CreateAsync(reportedChatUser, token);

            ReportModel reportModel = new ReportModel() 
            {
                ID = Guid.NewGuid(),
                ReportedUserID = reportedChatUser.ID,
                Title = "Title",
                Message = "Message"
            };
            
            DeletionStatus expectedStatus = DeletionStatus.Deleted;
            await _reportRepository.CreateAsync(reportModel, token); //add report which will be deleted
            // Act

            var actual = await _reportRepository.DeleteAsync(reportModel.ID,token);
            _databaseFixture.ClearDatabase();
            // Assert
            actual.Should().Be(expectedStatus);
        }

        [Fact]
        public async void DeleteAsync_DeleteInexistentReportModelFromDb_DeletionStatusIsNotExisted() 
        {
            // Arrange
            CancellationToken token = default;
            DeletionStatus expectedStatus = DeletionStatus.NotExisted;
            // Act
            var actual = await _reportRepository.DeleteAsync(Guid.NewGuid(), token);

            // Assert
            actual.Should().Be(expectedStatus);
        }

        [Fact]
        public async void ListAsync_GetOnePage_PaginatedResultParametersMatchesWithReportsListParameters() 
        {
            CancellationToken token = default;
            var totalReportsCount = 5;
            var listParameters = new ReportsListParameters()
            {
                PageNumber = 1,
                PageSize = 3,
                SortOrder = SortOrder.Asc,
                SortBy = ReportSort.ReportedUserID,
            };
           

            var reportedUsers = CreateRandomUsersList(totalReportsCount);
            var reports = CreateRandomReportsList(reportedUsers);
            var expectedPaginatedResult = new PaginatedResult<ReportModel, ReportSort>()
            {
                TotalCount = totalReportsCount,
                TotalPages = 2,
                PageNumber = 1,
                PageSize = 3,
                SortOrder = SortOrder.Asc,
                SortBy = ReportSort.ReportedUserID,
                Entities = reports.OrderBy(x => new SqlGuid(x.ReportedUserID)).Take(3).ToList(),
            };

            await AddReportsAndUsersToDatabase(reportedUsers, reports, token);

            //Act
            var actualPaginatedResult = await _reportRepository.ListAsync(listParameters, token);

            _databaseFixture.ClearDatabase();

            //Assert
            actualPaginatedResult.Should().BeEquivalentTo(expectedPaginatedResult);

        }

        [Fact]
        public async void ListAsync_GetSecondPageListSortByReportedUserIdOrderByAsc_Return2ReportsList()
        {
            //Arrange
            CancellationToken token = default;
            var expectedListParameters = new ReportsListParameters()
            {
                PageNumber = 2,
                PageSize = 3,
                SortOrder = SortOrder.Asc,
                SortBy = ReportSort.ReportedUserID,
            };

            var reportedUsers = CreateRandomUsersList(5);

            var reports = CreateRandomReportsList(reportedUsers);

            var expectedReportsList = reports.Select(x => new SqlGuid(x.ReportedUserID)).OrderBy(x => x).ToList();

            await AddReportsAndUsersToDatabase(reportedUsers,reports, token);

            //Act
            var actualReportsList = await _reportRepository.ListAsync(expectedListParameters, token);

            _databaseFixture.ClearDatabase();

            var expextedReportedUsersIdsSequence = expectedReportsList.Select(x => x.Value).TakeLast(2).ToList();
            var actualReportedUsersIdsSequence = actualReportsList.Entities.Select(x => x.ReportedUserID);

            //Assert
            Assert.True(actualReportedUsersIdsSequence.SequenceEqual(expextedReportedUsersIdsSequence));
        }

        [Fact]
        public async void ListAsync_GetFirstPageListSortByReportedUserIdOrderByAsc_Return3ReportsList()
        {
            //Arrange
            CancellationToken token = default;
            var expectedListParameters = new ReportsListParameters()
            {
                PageNumber = 1,
                PageSize = 3,
                SortOrder = SortOrder.Asc,
                SortBy = ReportSort.ReportedUserID,
            };

            var reportedUsers = CreateRandomUsersList(5);

            var reports = CreateRandomReportsList(reportedUsers);

            var expectedReportsList = reports.Select(x => new SqlGuid(x.ReportedUserID)).OrderBy(x => x).ToList();

            await AddReportsAndUsersToDatabase(reportedUsers, reports, token);

            //Act
            var actualReportsList = await _reportRepository.ListAsync(expectedListParameters, token);

            _databaseFixture.ClearDatabase();

            var expextedReportedUsersIdsSequence = expectedReportsList.Select(x => x.Value).Take(3).ToList();
            var actualReportedUsersIdsSequence = actualReportsList.Entities.Select(x => x.ReportedUserID);

            //Assert
            Assert.True(actualReportedUsersIdsSequence.SequenceEqual(expextedReportedUsersIdsSequence));
        }

        [Fact]
        public async void ListAsync_GetAllReportsListSortByReportedUserIdOrderByDesc_ReturnAllReportsList() 
        {
            //Arrange
            CancellationToken token = default;
            var expectedListParameters = new ReportsListParameters()
            {
                PageNumber = 1,
                PageSize = 10,
                SortOrder = SortOrder.Desc,
                SortBy = ReportSort.ReportedUserID,
            };

            var reportedUsers = CreateRandomUsersList(5);

            var reports = CreateRandomReportsList(reportedUsers);

            var expectedReportsList = reports.Select(x => new SqlGuid(x.ReportedUserID)).OrderByDescending(x => x).ToList();

            await AddReportsAndUsersToDatabase(reportedUsers, reports, token);

            //Act
            var actualReportsList = await _reportRepository.ListAsync(expectedListParameters, token);

            _databaseFixture.ClearDatabase();

            var actualListParameters = new ReportsListParameters()
            {
                PageNumber = actualReportsList.PageNumber,
                PageSize = actualReportsList.PageSize,
                SortOrder = actualReportsList.SortOrder,
                SortBy = actualReportsList.SortBy
            };

            var expextedReportedUsersIdsSequence = expectedReportsList.Select(x => x.Value).ToList();
            var actualReportedUsersIdsSequence = actualReportsList.Entities.Select(x => x.ReportedUserID);

            //Assert
            expectedListParameters.Should().BeEquivalentTo(actualListParameters);
            Assert.True(actualReportedUsersIdsSequence.SequenceEqual(expextedReportedUsersIdsSequence));
        }

        [Fact]
        public async void ListAsync_GetAllReportsListSortByReportedUserIdOrderByAsc_ReturnsAllReportsList()
        {
            //Arrange
            CancellationToken token = default;
            var expectedListParameters = new ReportsListParameters()
            {
                PageNumber = 1,
                PageSize = 10,
                SortOrder = SortOrder.Asc,
                SortBy = ReportSort.ReportedUserID,
            };

            var reportedUsers = CreateRandomUsersList(5);

            var reports = CreateRandomReportsList(reportedUsers);

            var expectedReportsList = reports.Select(x => new SqlGuid(x.ReportedUserID)).OrderBy(x => x).ToList();

            await AddReportsAndUsersToDatabase(reportedUsers, reports, token);

            //Act
            var actualReportsList = await _reportRepository.ListAsync(expectedListParameters,token);

            _databaseFixture.ClearDatabase();

            var actualListParameters = new ReportsListParameters() 
            {
                PageNumber = actualReportsList.PageNumber,
                PageSize = actualReportsList.PageSize,
                SortOrder = actualReportsList.SortOrder,
                SortBy = actualReportsList.SortBy
            };

            var expextedReportedUsersIdsSequence = expectedReportsList.Select(x => x.Value).ToList();
            var actualReportedUsersIdsSequence = actualReportsList.Entities.Select(x => x.ReportedUserID);

            //Assert
            expectedListParameters.Should().BeEquivalentTo(actualListParameters);
            Assert.True(actualReportedUsersIdsSequence.SequenceEqual(expextedReportedUsersIdsSequence));
        }

        private ChatUserModel CreateRandomChatUser()
        {
            return new ChatUserModel()
            {
                ID = Guid.NewGuid(),
                LastName = _fixture.Create<string>().Substring(0, 20),
                FirstName = _fixture.Create<string>().Substring(0, 20),
                Patronymic = _fixture.Create<string>().Substring(0, 20),
                Email = _fixture.Create<string>().Substring(0, 20),
                UniversityName = _fixture.Create<string>().Substring(0, 20),
                UniversityFaculty = _fixture.Create<string>().Substring(0, 20),
                JoinedUtc = _fixture.Create<DateTime>(),
                LastActiveUtc = _fixture.Create<DateTime>(),
                IsBlocked = _fixture.Create<bool>(),
                BlockedUntilUtc = _fixture.Create<DateTime>()
            };
        }
        private ReportModel CreateRandomReport(ChatUserModel reportedUser) 
        {
            return new ReportModel()
            {
                ID = Guid.NewGuid(),
                ReportedUserID = reportedUser.ID,
                Title = _fixture.Create<string>().Substring(0, 20),
                Message = _fixture.Create<string>().Substring(0, 20)
            };
        }

        private async Task AddReportsAndUsersToDatabase(List<ChatUserModel> users, List<ReportModel> reports, CancellationToken token) 
        {

            foreach (var user in users)
            {
                await _chatUserRepository.CreateAsync(user, token);
            }

            foreach (var report in reports)
            {
                await _reportRepository.CreateAsync(report, token);
            }
        }

        private List<ChatUserModel> CreateRandomUsersList(int count)
        {
            var list = new List<ChatUserModel>();
            for (int i = 0; i < count; i++)
            {
                list.Add(CreateRandomChatUser());
            }
            return list;
        }

        private List<ReportModel> CreateRandomReportsList(List<ChatUserModel> users)
        {
            var list = new List<ReportModel>();
            for (int i = 0; i < users.Count; i++)
            {
                list.Add(CreateRandomReport(users[i]));
            }
            return list;
        }
    }
}