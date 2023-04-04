using AutoFixture;
using Castle.Core.Smtp;
using Chatter.Security.Common.Enums;
using Chatter.Security.Core.Enums;
using Chatter.Security.Core.Interfaces;
using Chatter.Security.Core.Models;
using Chatter.Security.Core.Services;
using Chatter.Security.DataAccess.Interfaces;
using Chatter.Security.DataAccess.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data;

namespace Chatter.Security.Tests.UnitTests
{
    public class IdentityServiceTests
    {
        private readonly IdentityService _identityService;
        private readonly Fixture _fixture;
        private readonly Mock<IHMACEncryptor> _encryptorMock;
        private readonly Mock<IIdentityRepository> _identityRepositoryMock;
        private readonly Mock<IUserRoleRepository> _userRoleRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;

        public IdentityServiceTests()
        {
            _fixture = new Fixture();
            var loggerMock = new Mock<ILogger<IdentityService>>();
            _identityRepositoryMock = new Mock<IIdentityRepository>();
            _userRoleRepositoryMock = new Mock<IUserRoleRepository>();
            _configurationMock = new Mock<IConfiguration>();

            _encryptorMock = new Mock<IHMACEncryptor>();
            _encryptorMock.Setup(x => x.CreateRandomPasswordKey(It.IsAny<IConfiguration>()))
              .Returns(_fixture.Create<string>());
            _encryptorMock.Setup(x => x.EncryptPassword(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Returns(_fixture.Create<string>());

           
            _identityService = new IdentityService(_identityRepositoryMock.Object, _configurationMock.Object,
                _userRoleRepositoryMock.Object, _encryptorMock.Object, loggerMock.Object);

        }

        [Fact]
        public async void CreateAsync_CreateIdentityWithUniqueEmail_ReturnsCreatedIdentityID()
        {
            //Arrange
            CancellationToken token = default;
            var createModel = _fixture.Create<CreateIdentity>();

            //Act
            var actual = await _identityService.CreateAsync(createModel, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async void CreateAsync_CreateIdentityWithExistingEmail_ReturnsBusinesError()
        {
            //Arrange
            CancellationToken token = default;
            var createModel = _fixture.Create<CreateIdentity>();
            _identityRepositoryMock.Setup(x => x.GetByEmailOrUserTagAsync(It.IsAny<EmailOrUserTagSearchModel>(), token))
                .Returns(Task.FromResult(_fixture.Create<IdentityModel>()));

            //Act
            var actual = await _identityService.CreateAsync(createModel, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(ErrorType.BusinessError);
        }

        [Fact]
        public async void AddRoleToIdentityAsync_AddUniqueUserRoleToIdentity_ReturnsCreatedUserRoleID()
        {
            //Arrange
            CancellationToken token = default;
            var roleIdInDb = Guid.NewGuid();
            var userRole = _fixture.Create<UserRole>();

            _identityRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), token))
                .Returns(Task.FromResult(_fixture.Create<IdentityModel>()));

            _userRoleRepositoryMock.Setup(x => x.AddRoleToUserAsync(It.IsAny<Guid>(), It.IsAny<UserRole>(), token))
                .Returns(Task.FromResult(roleIdInDb));

            //Act
            var actual = await _identityService.AddRoleToIdentityAsync(roleIdInDb, userRole, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async void AddRoleToIdentityAsync_AddExistentUserRoleToIdentity_ReturnsCreatedUserRoleID()
        {
            //Arrange
            CancellationToken token = default;
            var roleIdInDb = Guid.NewGuid();
            var userRole = _fixture.Create<UserRole>();
            _userRoleRepositoryMock.Setup(x => x.GetRoleIdAsync(It.IsAny<Guid>(), It.IsAny<UserRole>(), token))
                .Returns(Task.FromResult(roleIdInDb));

            //Act
            var actual = await _identityService.AddRoleToIdentityAsync(roleIdInDb, userRole, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(ErrorType.BusinessError);
        }

        [Fact]
        public async void AddRoleToIdentityAsync_AddRoleToInexistentIdentity_ReturnsBusinessError()
        {
            //Arrange
            CancellationToken token = default;
            var userRole = _fixture.Create<UserRole>();

            //Act
            var actual = await _identityService.AddRoleToIdentityAsync(Guid.NewGuid(), userRole, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(ErrorType.BusinessError);
        }

        [Fact]
        public async void RemoveRoleIdentityAsync_RemoveExistingRoleFromIdentity_ReturnsRemovedRoleID() 
        {
            //Arrange
            CancellationToken token = default;
            var roleId = Guid.NewGuid();
            var role = _fixture.Create<UserRole>();

            _identityRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), token))
                .Returns(Task.FromResult(_fixture.Create<IdentityModel>()));

            _userRoleRepositoryMock.Setup(x => x.GetRoleIdAsync(It.IsAny<Guid>(), It.IsAny<UserRole>(), token))
                .Returns(Task.FromResult(roleId));

            _userRoleRepositoryMock.Setup(x => x.DeleteUserRoleAsync(It.IsAny<Guid>(), It.IsAny<UserRole>(), token))
                .Returns(Task.FromResult(DeletionStatus.Deleted));

            //Act
            var actual = await _identityService.RemoveRoleIdentityAsync(Guid.NewGuid(), role, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Should().Be(roleId);
        }

        [Fact]
        public async void RemoveRoleIdentityAsync_RemoveInexitentRoleFromIdentity_ReturnsBusinessError()
        {
            //Arrange
            CancellationToken token = default;
            var roleId = Guid.NewGuid();
            var role = _fixture.Create<UserRole>();

            _identityRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), token))
                .Returns(Task.FromResult(_fixture.Create<IdentityModel>()));

            _userRoleRepositoryMock.Setup(x => x.DeleteUserRoleAsync(It.IsAny<Guid>(), It.IsAny<UserRole>(), token))
                .Returns(Task.FromResult(DeletionStatus.NotExisted));

            //Act
            var actual = await _identityService.RemoveRoleIdentityAsync(Guid.NewGuid(), role, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(ErrorType.BusinessError);
        }

        [Fact]
        public async void RemoveRoleIdentityAsync_RemoveRoleFromInexistentIdentity_ReturnsBusinessError()
        {
            //Arrange
            CancellationToken token = default;
            var role = _fixture.Create<UserRole>();

            //Act
            var actual = await _identityService.RemoveRoleIdentityAsync(Guid.NewGuid(), role, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(ErrorType.BusinessError);
        }

        [Fact]
        public async void UpdateAsync_UpdateExistingIdentity_ReturnsUpdatedIdentityID() 
        {
            //Arrange
            CancellationToken token = default;
            _identityRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<UpdateIdentityModel>(), token))
                .Returns(Task.FromResult(true));

            var updateModel = _fixture.Create<UpdateIdentity>();

            //Act
            var actual = await _identityService.UpdateAsync(updateModel, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Should().Be(updateModel.ID);
        }

        [Fact]
        public async void UpdateAsync_UpdateInexistentIdentity_ReturnsBusinessError()
        {
            //Arrange
            CancellationToken token = default;
            _identityRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<UpdateIdentityModel>(), token))
                .Returns(Task.FromResult(false));

            var updateModel = _fixture.Create<UpdateIdentity>();

            //Act
            var actual = await _identityService.UpdateAsync(updateModel, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(ErrorType.BusinessError);
        }

        [Fact]
        public async void DeleteAsync_DeleteExistingIdentity_ReturnsDeletedIdentityID() 
        {
            //Arrange
            CancellationToken token = default;
            var identityID = Guid.NewGuid();
            _identityRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<Guid>(), token))
                .Returns(Task.FromResult(DeletionStatus.Deleted));

            //Act
            var actual = await _identityService.DeleteAsync(identityID, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Should().Be(identityID);
        }

        [Fact]
        public async void DeleteAsync_DeleteInexistentIndentity_ReturnsBusinessError() 
        {
            //Arrange
            CancellationToken token = default;
            var identityID = Guid.NewGuid();
            _identityRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<Guid>(), token))
                .Returns(Task.FromResult(DeletionStatus.NotExisted));

            //Act
            var actual = await _identityService.DeleteAsync(identityID, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(ErrorType.BusinessError);
        }

        [Fact]
        public async void FindByIdAsync_FindExistingIdentityById_ReturnsIdentityWithSpecifiedId() 
        {
            //Arrange
            CancellationToken token = default;
            var identity = _fixture.Create<IdentityModel>();
            _identityRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), token))
             .Returns(Task.FromResult(identity));

            var expected = new Identity() 
            {
                ID = identity.ID,
                Email = identity.Email,
                UserTag = identity.UserTag,
                PasswordHash = identity.PasswordHash,
                PasswordKey = identity.PasswordKey,
                UserID = identity.UserID
            };

            //Act
            var actual = await _identityService.FindByIdAsync(identity.ID, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void FindByIdAsync_FindInexistentIdentityById_ReturnsBusinessError()
        {
            //Arrange
            CancellationToken token = default;
            var identity = _fixture.Create<IdentityModel>();

            //Act
            var actual = await _identityService.FindByIdAsync(identity.ID, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(ErrorType.BusinessError);
        }

        [Fact]
        public async void FindByEmailAsync_FindExistingIdentityByEmail_ReturnsIdentityWithSpecifiedEmail()
        {
            //Arrange
            CancellationToken token = default;
            var identity = _fixture.Create<IdentityModel>();
            _identityRepositoryMock.Setup(x => x.GetByEmailOrUserTagAsync(It.IsAny<EmailOrUserTagSearchModel>(), token))
             .Returns(Task.FromResult(identity));

            var expected = new Identity()
            {
                ID = identity.ID,
                Email = identity.Email,
                UserTag = identity.UserTag,
                PasswordHash = identity.PasswordHash,
                PasswordKey = identity.PasswordKey,
                UserID = identity.UserID
            };

            //Act
            var actual = await _identityService.FindByEmailAsync(identity.Email, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void FindByEmailAsync_FindInexistentIdentityById_ReturnsBusinessError()
        {
            //Arrange
            CancellationToken token = default;
            var identity = _fixture.Create<IdentityModel>();

            //Act
            var actual = await _identityService.FindByEmailAsync(identity.Email, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(ErrorType.BusinessError);
        }

        [Fact]
        public async void FindByUserTagAsync_FindExistingIdentityByUserTag_ReturnsIdentityWithSpecifiedUserTag()
        {
            //Arrange
            CancellationToken token = default;
            var identity = _fixture.Create<IdentityModel>();
            _identityRepositoryMock.Setup(x => x.GetByEmailOrUserTagAsync(It.IsAny<EmailOrUserTagSearchModel>(), token))
             .Returns(Task.FromResult(identity));

            var expected = new Identity()
            {
                ID = identity.ID,
                Email = identity.Email,
                UserTag = identity.UserTag,
                PasswordHash = identity.PasswordHash,
                PasswordKey = identity.PasswordKey,
                UserID = identity.UserID
            };

            //Act
            var actual = await _identityService.FindByUserTagAsync(identity.UserTag, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void FindByUserTagAsync_FindInexistentIdentityByUserTag_ReturnsBusinessError()
        {
            //Arrange
            CancellationToken token = default;
            var identity = _fixture.Create<IdentityModel>();

            //Act
            var actual = await _identityService.FindByUserTagAsync(identity.UserTag, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(ErrorType.BusinessError);
        }

        [Fact]
        public async void GetRolesAsync_GetRolesFromExistingIdentity_ReturnsIdentityRolesList() 
        {
            //Arrange
            CancellationToken token = default;
            var identity = _fixture.Create<IdentityModel>();
            var rolesFromDb = _fixture.Create<IList<UserRole>>();
            var expected = rolesFromDb.Select(x => x.ToString()).ToList();
            _userRoleRepositoryMock.Setup(x => x.GetUserRolesAsync(It.IsAny<Guid>(), token))
            .Returns(Task.FromResult(rolesFromDb));

            _identityRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>(), token))
            .Returns(Task.FromResult(identity));

            //Act
            var actual = await _identityService.GetRolesAsync(identity.ID,token);

            //Arrange
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void GetRolesAsync_GetRolesFromInexistentIdentity_ReturnsBusinessError()
        {
            //Arrange
            CancellationToken token = default;

            //Act
            var actual = await _identityService.GetRolesAsync(Guid.NewGuid(), token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(ErrorType.BusinessError);
        }
    }
}