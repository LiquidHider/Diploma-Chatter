using AutoFixture;
using Chatter.Domain.BusinessLogic.Models;
using Chatter.Domain.BusinessLogic.Models.Create;
using Chatter.Domain.BusinessLogic.Services;
using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.Interfaces;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.DataAccess.Models.Parameters;
using Chatter.Domain.Tests.Common;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Chatter.Domain.Tests.UnitTests
{
    public class ReportServiceTests
    {
        private readonly ReportService _reportService;
        private readonly Fixture _fixture;
        private readonly Mock<IChatUserRepository> _chatUserRepositoryMock;
        private readonly Mock<IReportRepository> _reportRepositoryMock;
        private readonly ChatUserFixtureHelper _chatUserFixtureHelper;
        private readonly ReportFixtureHelper _reportFixtureHelper;

        public ReportServiceTests()
        {
            var reportServiceLoggerMock = new Mock<ILogger<ReportService>>();
            _reportRepositoryMock = new Mock<IReportRepository>();
            _chatUserRepositoryMock = new Mock<IChatUserRepository>();

            _chatUserFixtureHelper = new ChatUserFixtureHelper();
            _reportFixtureHelper = new ReportFixtureHelper();

            _reportService = new ReportService(_reportRepositoryMock.Object,
                _chatUserRepositoryMock.Object,
                reportServiceLoggerMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async void CreateReport_CreateNewReportWithExistedUser_ReturnsServiseResultWithCreatedReport()
        {
            //Arrange
            CancellationToken token = default;
            var userModelFromDb = _chatUserFixtureHelper.CreateRandomChatUser();
            var userID = userModelFromDb.ID;

            _chatUserRepositoryMock.Setup(x => x.GetAsync(userID, token))
            .Returns(Task.FromResult(userModelFromDb));

            var createModel = new CreateReport()
            {
                Title = _fixture.Create<string>(),
                Message = _fixture.Create<string>(),
                ReportedUserID = userModelFromDb.ID

            };
            createModel.ReportedUserID = userID;

            //Act
            var actual = await _reportService.CreateReportAsync(createModel, token);
            var expected = new Report()
            {
                ID = actual.Value.ID,
                Title = createModel.Title,
                Message = createModel.Message,
                ReportedUserID = createModel.ReportedUserID,
            };

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Should().NotBeNull();
            actual.Value.Should().NotBeNull();
            actual.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void CreateReport_CreateNewReportWithInexistentUser_ReturnsServiseResultWithError()
        {
            //Arrange
            CancellationToken token = default;
            var createModel = new CreateReport()
            {
                Title = _fixture.Create<string>(),
                Message = _fixture.Create<string>(),
                ReportedUserID = Guid.NewGuid()
            };

            //Act
            var actual = await _reportService.CreateReportAsync(createModel, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Value.Should().BeNull();
            actual.IsEmpty.Should().BeTrue();
            actual.Error.Type.Should().Be(BusinessLogic.Enums.ErrorType.BusinessError);
        }

        [Fact]
        public async void CreateReport_CreateNewReportWithExistedUser_ReturnsServiseResultWithException()
        {
            //Arrange
            CancellationToken token = default;

            //Act
            var actual = await _reportService.CreateReportAsync(null, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Value.Should().BeNull();
            actual.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public async void RemoveReport_RemoveExistedReport_ReturnsServiceResultWithRemovedReportId()
        {
            //Arrange
            CancellationToken token = default;
            var reportId = Guid.NewGuid();
            _reportRepositoryMock.Setup(x => x.DeleteAsync(reportId, token))
             .Returns(Task.FromResult(DeletionStatus.Deleted));

            //Act
            var actual = await _reportService.RemoveReportAsync(reportId, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.IsEmpty.Should().BeFalse();
            actual.Value.Should().Be(reportId);
        }

        [Fact]
        public async void RemoveReport_RemoveInexistentReport_ReturnsServiceResultWithError()
        {
            //Arrange
            CancellationToken token = default;
            var reportId = Guid.NewGuid();
            _reportRepositoryMock.Setup(x => x.DeleteAsync(reportId, token))
             .Returns(Task.FromResult(DeletionStatus.NotExisted));

            //Act
            var actual = await _reportService.RemoveReportAsync(reportId, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(BusinessLogic.Enums.ErrorType.BusinessError);
        }

        [Fact]
        public async void SendReport_SendExistedReportInDb_ReturnsServiceResultWithReportID()
        {
            //Arrange
            CancellationToken token = default;
            var userModel = _chatUserFixtureHelper.CreateRandomChatUser();

            var report = new Report()
            {
                ID = Guid.NewGuid(),
                ReportedUserID = userModel.ID,
                Title = _fixture.Create<string>().Substring(0, 20),
                Message = _fixture.Create<string>()
            };

            _reportRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<ReportModel>(), token))
            .Returns(Task.CompletedTask);

            _chatUserRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), token))
            .Returns(Task.FromResult(userModel));

            //Act
            var actual = await _reportService.SendReportAsync(report, token);
            var expected = report.ID;

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.IsEmpty.Should().BeFalse();
            actual.Value.Should().Be(expected);
        }

        [Fact]
        public async void SendReport_SendInexistentReport_ReturnsServiceResultWithError() 
        {
            //Arrange
            CancellationToken token = default;
            var report = _fixture.Create<Report>();

            //Act
            var actual = await _reportService.SendReportAsync(report, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(BusinessLogic.Enums.ErrorType.BusinessError);
        }

        [Fact]
        public async void GetReportsListAsync_GetFirstPageOfFiveReports_ReturnsServiceResultWithPaginatedResult()
        {
            //Arrange
            CancellationToken token = default;
            var totalReportsCount = 3;
            var listParameters = new ReportsListParameters()
            {
                PageNumber = 1,
                PageSize = 3,
                SortOrder = _fixture.Create<SortOrder>(),
                SortBy = _fixture.Create<ReportSort>()
            };

            var paginatedResultFromDb = new DataAccess.Models.Pagination.PaginatedResult<ReportModel, ReportSort>()
            {
                TotalCount = 5,
                TotalPages = 2,
                PageNumber = listParameters.PageNumber,
                PageSize = listParameters.PageSize,
                SortOrder = listParameters.SortOrder,
                SortBy = listParameters.SortBy,
                Entities = _reportFixtureHelper.CreateRandomReportsList(_chatUserFixtureHelper.CreateRandomUsersList(totalReportsCount))
            };

            _reportRepositoryMock.Setup(x => x.ListAsync(It.IsAny<ReportsListParameters>(), token))
            .Returns(Task.FromResult(paginatedResultFromDb));

            //Act
            var actual = await _reportService.GetReportsListAsync(listParameters, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Should().NotBeNull();
            actual.Value.Value.Count.Should().Be(listParameters.PageSize);
            actual.Value.Value.Should().NotBeEmpty();
            actual.Value.Value.Should().NotContainNulls();
        }
    }
}