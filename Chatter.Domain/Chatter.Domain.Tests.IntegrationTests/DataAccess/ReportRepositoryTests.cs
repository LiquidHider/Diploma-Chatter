using AutoFixture;
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
using System.Data.SqlTypes;

namespace Chatter.Domain.Tests.IntegrationTests.DataAccess
{
    public class ReportRepositoryTests
    {
        private readonly ReportRepository _reportRepository;
        private readonly ChatUserRepository _chatUserRepository;
        private readonly DatabaseFixture _databaseFixture;
        private readonly ChatUserFixtureHelper _chatUserFixtureHelper;
        private readonly ReportFixtureHelper _reportFixtureHelper;

        public ReportRepositoryTests()
        {
            _databaseFixture = new DatabaseFixture();
            _databaseFixture.EnsureCreated(true);
            _chatUserFixtureHelper = new ChatUserFixtureHelper();
            _reportFixtureHelper = new ReportFixtureHelper();
            var reportRepoLoggerMock = new Mock<ILogger<ReportRepository>>();
            var chatUserRepoLoggerMock = new Mock<ILogger<ChatUserRepository>>();

            var optionsMock = new Mock<IOptions<DatabaseOptions>>();
            optionsMock.Setup(x => x.Value)
            .Returns(_databaseFixture.dbOptions);

            _reportRepository = new ReportRepository(optionsMock.Object, reportRepoLoggerMock.Object);
            _chatUserRepository = new ChatUserRepository(optionsMock.Object, chatUserRepoLoggerMock.Object);
        }

        [Fact]
        public async void GetAsync_GetExistedReportInDb_ReturnsReportModel()
        {
            //Arrange
            CancellationToken token = default;
            ChatUserModel reportedChatUserModel = _chatUserFixtureHelper.CreateRandomChatUser();
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
            await _databaseFixture.ClearDatabaseAsync();
            //Assert
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(reportModel);
        }

        [Fact]
        public async void GetAsync_GetInexistentReportInDb_ReturnsNull()
        {
            //Arrange
            CancellationToken token = default;

            //Act
            var actual = await _reportRepository.GetAsync(Guid.NewGuid(), token);

            //Assert
            actual.Should().BeNull();
        }


        [Fact]
        public async void DeleteAsync_DeleteExistedReportModelFromDb_DeletionStatusIsDeleted()
        {
            // Arrange
            CancellationToken token = default;
            ChatUserModel reportedChatUser = _chatUserFixtureHelper.CreateRandomChatUser();
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

            var actual = await _reportRepository.DeleteAsync(reportModel.ID, token);
            await _databaseFixture.ClearDatabaseAsync();
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
        public async void ListAsync_SendNullListParameters_ThrowsArgumentNullException() 
        {
            //Arrange
            CancellationToken token = default;
            ReportsListParameters listParameters = null;

            //Assert
            Assert.ThrowsAsync<ArgumentNullException>(
                () =>  _reportRepository.ListAsync(listParameters, token));
        }

        [Fact]
        public async void ListAsync_GetFirstPage_PaginatedResultParametersMatchesExpectedListParameters()
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


            var reportedUsers = _chatUserFixtureHelper.CreateRandomUsersList(totalReportsCount);
            var reports = _reportFixtureHelper.CreateRandomReportsList(reportedUsers);
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

            await _databaseFixture.ClearDatabaseAsync();

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

            var reportedUsers = _chatUserFixtureHelper.CreateRandomUsersList(5);

            var reports = _reportFixtureHelper.CreateRandomReportsList(reportedUsers);

            var expectedReportsList = reports.Select(x => new SqlGuid(x.ReportedUserID)).OrderBy(x => x).ToList();

            await AddReportsAndUsersToDatabase(reportedUsers, reports, token);

            //Act
            var actualReportsList = await _reportRepository.ListAsync(expectedListParameters, token);

            await _databaseFixture.ClearDatabaseAsync();

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

            var reportedUsers = _chatUserFixtureHelper.CreateRandomUsersList(5);

            var reports = _reportFixtureHelper.CreateRandomReportsList(reportedUsers);

            var expectedReportsList = reports.Select(x => new SqlGuid(x.ReportedUserID)).OrderBy(x => x).ToList();

            await AddReportsAndUsersToDatabase(reportedUsers, reports, token);

            //Act
            var actualReportsList = await _reportRepository.ListAsync(expectedListParameters, token);

            await _databaseFixture.ClearDatabaseAsync();

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

            var reportedUsers = _chatUserFixtureHelper.CreateRandomUsersList(5);

            var reports = _reportFixtureHelper.CreateRandomReportsList(reportedUsers);

            var expectedReportsList = reports.Select(x => new SqlGuid(x.ReportedUserID)).OrderByDescending(x => x).ToList();

            await AddReportsAndUsersToDatabase(reportedUsers, reports, token);

            //Act
            var actualReportsList = await _reportRepository.ListAsync(expectedListParameters, token);

            await _databaseFixture.ClearDatabaseAsync();

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

            var reportedUsers = _chatUserFixtureHelper.CreateRandomUsersList(5);

            var reports = _reportFixtureHelper.CreateRandomReportsList(reportedUsers);

            var expectedReportsList = reports.Select(x => new SqlGuid(x.ReportedUserID)).OrderBy(x => x).ToList();

            await AddReportsAndUsersToDatabase(reportedUsers, reports, token);

            //Act
            var actualReportsList = await _reportRepository.ListAsync(expectedListParameters, token);

            await _databaseFixture.ClearDatabaseAsync();

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
        public async void ListAsync_GetExistedReportssWithSpecificReportedUsersIDs_ReturnsExpectedPaginatedResult() 
        {
            //Arrange
            CancellationToken token = default;
            var reportedUsers = _chatUserFixtureHelper.CreateRandomUsersList(5);
            var expectedPhotosCount = 6;
            var listParameters = new ReportsListParameters()
            {
                PageNumber = 1,
                PageSize = 10,
                SortOrder = SortOrder.Desc,
                SortBy = ReportSort.ReportedUserID,
                ReportedUsersIDs = new List<Guid>()
                {
                    reportedUsers[0].ID,
                    reportedUsers[1].ID,
                },
            };

            var reports = _reportFixtureHelper.CreateRandomReportsList(reportedUsers);

            for (int i = 0; i < 2; i++)
            {
                reports.Add(_reportFixtureHelper.CreateRandomReport(reportedUsers[0]));
                reports.Add(_reportFixtureHelper.CreateRandomReport(reportedUsers[1]));
            }

            await AddReportsAndUsersToDatabase(reportedUsers, reports, token);

            //Act
            var actualPaginatedResult = await _reportRepository.ListAsync(listParameters, token);

            await _databaseFixture.ClearDatabaseAsync();

            var expectedPaginatedResult = new PaginatedResult<ReportModel, ReportSort>() 
            {
                TotalCount = expectedPhotosCount,
                TotalPages = 1,
                PageNumber = 1,
                PageSize = 10,
                SortOrder = SortOrder.Desc,
                SortBy = ReportSort.ReportedUserID,
                Entities = reports.Where(x => x.ReportedUserID == reportedUsers[0].ID || x.ReportedUserID == reportedUsers[1].ID)
                .OrderByDescending(x => new SqlGuid(x.ReportedUserID)).ToList(),
            };

            var expextedReportedUsersIdsSequence = expectedPaginatedResult.Entities.Select(x => x.ReportedUserID).ToList();
            var actualReportedUsersIdsSequence = actualPaginatedResult.Entities.Select(x => x.ReportedUserID).ToList();

            //Assert
            actualPaginatedResult.Should().BeEquivalentTo(expectedPaginatedResult, o => o.Excluding(x => x.Entities));
            Assert.True(actualReportedUsersIdsSequence.SequenceEqual(expextedReportedUsersIdsSequence));
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
    }
}