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
using Xunit;

namespace Chatter.Domain.Tests.IntegrationTests.DataAccess
{
    public class PhotoRepositoryTests
    {
        private readonly PhotoRepository _photoRepository;
        private readonly ChatUserRepository _chatUserRepository;
        private readonly DatabaseFixture _databaseFixture;
        private readonly PhotoFixtureHelper _photoFixtureHelper;
        private readonly ChatUserFixtureHelper _chatUserFixtureHelper;


        public PhotoRepositoryTests()
        {
            _databaseFixture = new DatabaseFixture();
            _databaseFixture.EnsureCreated(false);
            _photoFixtureHelper = new PhotoFixtureHelper();
            _chatUserFixtureHelper = new ChatUserFixtureHelper();
            var reportRepoLoggerMock = new Mock<ILogger<PhotoRepository>>();
            var chatUserRepoLoggerMock = new Mock<ILogger<ChatUserRepository>>();

            var optionsMock = new Mock<IOptions<DatabaseOptions>>();
            optionsMock.Setup(x => x.Value)
            .Returns(_databaseFixture.dbOptions);

            _photoRepository = new PhotoRepository(reportRepoLoggerMock.Object, optionsMock.Object);
            _chatUserRepository = new ChatUserRepository(optionsMock.Object, chatUserRepoLoggerMock.Object);
        }

        [Fact]
        public async void GetAsync_GetExistedPhotoInDb_ReturnsPhotoModel()
        {
            //Arrange
            CancellationToken token = default;
            ChatUserModel photoOwner = _chatUserFixtureHelper.CreateRandomChatUser();
            await _chatUserRepository.CreateAsync(photoOwner, token);

            var expectedModel = _photoFixtureHelper.CreateRandomPhoto(photoOwner);
            await _photoRepository.CreateAsync(expectedModel, token);

            //Act
            var actual = await _photoRepository.GetAsync(expectedModel.ID, token);

            await _databaseFixture.ClearDatabaseAsync();
            //Assert
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(expectedModel);
        }

        [Fact]
        public async void GetAsync_GetInexistentPhotoInDb_ThrowsInvalidOperationException()
        {
            //Arrange
            CancellationToken token = default;

            //Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                 () => _photoRepository.GetAsync(Guid.NewGuid(), token));
        }

        [Fact]
        public async void DeleteAsync_DeleteExistedPhotoFromDb_DeletionStatusIsDeleted()
        {
            //Arrange
            CancellationToken token = default;
            ChatUserModel photoOwner = _chatUserFixtureHelper.CreateRandomChatUser();
            await _databaseFixture.ClearDatabaseAsync();
            await _chatUserRepository.CreateAsync(photoOwner, token); //add photo owner to db

            var modelToDelete = _photoFixtureHelper.CreateRandomPhoto(photoOwner);
            await _photoRepository.CreateAsync(modelToDelete, token); //add photo to db which will be deleted
            DeletionStatus expectedDeletionStatus = DeletionStatus.Deleted;

            //Act
            var actualDeletionStatus = await _photoRepository.DeleteAsync(modelToDelete.ID, token);

            await _databaseFixture.ClearDatabaseAsync();
            //Assert
            actualDeletionStatus.Should().Be(expectedDeletionStatus);
        }

        [Fact]
        public async void DeleteAsync_DeleteInexistentPhotoFromDb_DeletionStatusIsNotExisted()
        {
            //Arrange
            CancellationToken token = default;
            DeletionStatus expectedStatus = DeletionStatus.NotExisted;

            //Act
            var actual = await _photoRepository.DeleteAsync(Guid.NewGuid(), token);

            await _databaseFixture.ClearDatabaseAsync();
            //Assert
            actual.Should().Be(expectedStatus);
        }

        [Fact]
        public async void ListAsync_SendNullListParameters_ThrowsArgumentNullException()
        {
            //Arrange
            CancellationToken token = default;
            PhotosListParameters listParameters = null;

            //Assert
            Assert.ThrowsAsync<ArgumentNullException>(
                () => _photoRepository.ListAsync(listParameters, token));
        }

        [Fact]
        public async void ListAsync_GetFirstPage_PaginatedResultParametersMatchesWithExpected()
        {
            //Arrange
            CancellationToken token = default;
            var totalReportsCount = 5;
            var listParameters = new PhotosListParameters()
            {
                PageNumber = 1,
                PageSize = 3,
                SortOrder = SortOrder.Asc,
                SortBy = PhotoSort.IsMain,
            };

            var photosOwners = _chatUserFixtureHelper.CreateRandomUsersList(totalReportsCount);
            var photos = _photoFixtureHelper.CreateRandomReportsList(photosOwners);
            var expectedPaginatedResult = new PaginatedResult<PhotoModel, PhotoSort>()
            {
                TotalCount = totalReportsCount,
                TotalPages = 2,
                PageNumber = 1,
                PageSize = 3,
                SortOrder = SortOrder.Asc,
                SortBy = PhotoSort.IsMain,
                Entities = photos.OrderBy(x => x.IsMain).Take(3).ToList(),
            };

            await AddPhotosAndUsersToDatabase(photosOwners, photos, token);

            //Act
            var actualPaginatedResult = await _photoRepository.ListAsync(listParameters, token);
            await _databaseFixture.ClearDatabaseAsync();

            var expectedBooleanSequence = expectedPaginatedResult.Entities.Select(x => x.IsMain).ToList();
            var actualBooleanSequence = actualPaginatedResult.Entities.Select(x => x.IsMain).ToList();

            //Assert
            actualPaginatedResult.Should().BeEquivalentTo(expectedPaginatedResult, o => o.Excluding(x => x.Entities));
            actualBooleanSequence.Should().BeEquivalentTo(expectedBooleanSequence);
        }

        [Fact]
        public async void ListAsync_GetSecondPageListSortByIsMainOrderByDesc_PaginatedResultMatchesWithExpected()
        {
            CancellationToken token = default;
            var totalReportsCount = 5;
            var listParameters = new PhotosListParameters()
            {
                PageNumber = 2,
                PageSize = 3,
                SortOrder = SortOrder.Desc,
                SortBy = PhotoSort.UserID,
            };

            var photosOwners = _chatUserFixtureHelper.CreateRandomUsersList(totalReportsCount);
            var photos = _photoFixtureHelper.CreateRandomReportsList(photosOwners);
            var expectedPaginatedResult = new PaginatedResult<PhotoModel, PhotoSort>()
            {
                TotalCount = totalReportsCount,
                TotalPages = 2,
                PageNumber = 2,
                PageSize = 3,
                SortOrder = SortOrder.Desc,
                SortBy = PhotoSort.UserID,
                Entities = photos.OrderByDescending(x => new SqlGuid(x.UserID)).TakeLast(2).ToList(),
            };
            await AddPhotosAndUsersToDatabase(photosOwners, photos, token);

            //Act
            var actualPaginatedResult = await _photoRepository.ListAsync(listParameters, token);

            await _databaseFixture.ClearDatabaseAsync();
            //Assert
            actualPaginatedResult.Should().BeEquivalentTo(expectedPaginatedResult);
        }

        [Fact]
        public async void ListAsync_GetAllReportsListSortByIsMainSortOrderAsc_PaginatedResultMatchesExpected() 
        {
            CancellationToken token = default;
            var totalReportsCount = 5;
            var listParameters = new PhotosListParameters()
            {
                PageNumber = 1,
                PageSize = 10,
                SortOrder = SortOrder.Asc,
                SortBy = PhotoSort.IsMain,
            };

            var photosOwners = _chatUserFixtureHelper.CreateRandomUsersList(totalReportsCount);
            var photos = _photoFixtureHelper.CreateRandomReportsList(photosOwners);
            var expectedPaginatedResult = new PaginatedResult<PhotoModel, PhotoSort>()
            {
                TotalCount = totalReportsCount,
                TotalPages = 1,
                PageNumber = 1,
                PageSize = 10,
                SortOrder = SortOrder.Asc,
                SortBy = PhotoSort.IsMain,
                Entities = photos.OrderBy(x => x.IsMain).ToList(),
            };
            await AddPhotosAndUsersToDatabase(photosOwners, photos, token);

            //Act
            var actualPaginatedResult = await _photoRepository.ListAsync(listParameters, token);

            await _databaseFixture.ClearDatabaseAsync();

            //Assert
            actualPaginatedResult.Should().BeEquivalentTo(expectedPaginatedResult);
        }

        [Fact]
        public async void ListAsync_GetAllReportsListFromEmptyDbSortByIsMainSortOrderAsc_PaginatedResultEntitiesAreNull()
        {
            CancellationToken token = default;
            var listParameters = new PhotosListParameters()
            {
                PageNumber = 1,
                PageSize = 10,
                SortOrder = SortOrder.Asc,
                SortBy = PhotoSort.IsMain,
            };

            var expectedPaginatedResult = new PaginatedResult<PhotoModel, PhotoSort>()
            {
                TotalCount = 0,
                TotalPages = 0,
                PageNumber = 1,
                PageSize = 10,
                SortOrder = SortOrder.Asc,
                SortBy = PhotoSort.IsMain,
                Entities = new List<PhotoModel>(),
            };

            //Act
            var actualPaginatedResult = await _photoRepository.ListAsync(listParameters, token);

            await _databaseFixture.ClearDatabaseAsync();
            var expectedIsMainFieldSequence = expectedPaginatedResult.Entities.Select(x => x.IsMain);
            var actualIsMainFieldSequence = actualPaginatedResult.Entities.Select(x => x.IsMain);

            //Assert
            actualPaginatedResult.Should().BeEquivalentTo(expectedPaginatedResult, o => o.Excluding(x => x.Entities));
            actualIsMainFieldSequence.Should().BeEquivalentTo(expectedIsMainFieldSequence);
        }

        [Fact]
        private async void ListAsync_GetExistedPhotosWithSpecificUserIDs_ReturnsExpectedPaginatedResult() 
        {
            CancellationToken token = default;
            var totalPhotosCount = 5;
            var expectedPhotosCount = 6;
            var photosOwners = _chatUserFixtureHelper.CreateRandomUsersList(totalPhotosCount);
            var photos = _photoFixtureHelper.CreateRandomReportsList(photosOwners);

            for (int i = 0; i < 2; i++)
            {
               photos.Add(_photoFixtureHelper.CreateRandomPhoto(photosOwners[0]));
               photos.Add(_photoFixtureHelper.CreateRandomPhoto(photosOwners[1]));
            }

            var listParameters = new PhotosListParameters()
            {
                PageNumber = 1,
                PageSize = 10,
                SortOrder = SortOrder.Asc,
                SortBy = PhotoSort.IsMain,
                UsersIDs = new List<Guid>() 
                {
                    photosOwners[0].ID, 
                    photosOwners[1].ID,
                },
            };

            
            var expectedPaginatedResult = new PaginatedResult<PhotoModel, PhotoSort>()
            {
                TotalCount = expectedPhotosCount,
                TotalPages = 1,
                PageNumber = 1,
                PageSize = 10,
                SortOrder = SortOrder.Asc,
                SortBy = PhotoSort.IsMain,
                Entities = photos
                .Where(x => x.UserID == photosOwners[0].ID || x.UserID == photosOwners[1].ID)
                .OrderBy(x => x.IsMain).ToList(),
            };
            await AddPhotosAndUsersToDatabase(photosOwners, photos, token);

            //Act
            var actualPaginatedResult = await _photoRepository.ListAsync(listParameters, token);

            await _databaseFixture.ClearDatabaseAsync();
            var expectedUserIdFieldSequence = expectedPaginatedResult.Entities.Select(x => x.UserID);
            var actualUserIdFieldSequence = actualPaginatedResult.Entities.Select(x => x.UserID);
            //Assert
            actualPaginatedResult.Should().BeEquivalentTo(expectedPaginatedResult);
            actualUserIdFieldSequence.Should().BeEquivalentTo(expectedUserIdFieldSequence);
        }

        private async Task AddPhotosAndUsersToDatabase(List<ChatUserModel> users, List<PhotoModel> photos, CancellationToken token)
        {

            foreach (var user in users)
            {
                await _chatUserRepository.CreateAsync(user, token);
            }

            foreach (var photo in photos)
            {
                await _photoRepository.CreateAsync(photo, token);
            }
        }
    }
}
