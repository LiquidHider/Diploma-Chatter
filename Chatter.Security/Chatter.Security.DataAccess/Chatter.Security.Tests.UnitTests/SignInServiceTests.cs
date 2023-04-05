using AutoFixture;
using Chatter.Security.Common;
using Chatter.Security.Core.Interfaces;
using Chatter.Security.Core.Models;
using Chatter.Security.Core.Services;
using Chatter.Security.DataAccess.Interfaces;
using Chatter.Security.DataAccess.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Chatter.Security.Tests.UnitTests
{
    public class SignInServiceTests
    {
        private readonly SignInService _signInService;
        private readonly Fixture _fixture;
        private readonly Mock<IIdentityRepository> _identityRepositoryMock;
        private readonly Mock<IHMACEncryptor> _encryptorMock;
        private readonly Mock<ILogger<SignInService>> _loggerMock;

        public SignInServiceTests()
        {
            _fixture = new Fixture();
            _identityRepositoryMock = new Mock<IIdentityRepository>();
            _loggerMock = new Mock<ILogger<SignInService>>();
            _encryptorMock = new Mock<IHMACEncryptor>();

            _signInService = new SignInService(_identityRepositoryMock.Object,_encryptorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async void SignInAsync_SuccessfullySignIn_ReturnsClaimsPrincipal()
        {
            //Arrange
            CancellationToken token = default;
            var signInModel = _fixture.Create<SignInModel>();
            var identity = _fixture.Create<IdentityModel>();
            var password = _fixture.Create<string>();
            signInModel.Password = password;
            identity.PasswordHash = password;

            _identityRepositoryMock.Setup(x => x.GetByEmailOrUserTagAsync(It.IsAny<EmailOrUserTagSearchModel>(), token))
                .Returns(Task.FromResult(identity));

            _encryptorMock.Setup(x => x.EncryptPassword(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Returns(password);

            _encryptorMock.Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>()))
               .Returns(true);

            //Act
            var actual = await _signInService.SignInAsync(signInModel, token);

            //Assert
            actual.IsSuccessful.Should().BeTrue();
            actual.Value.Should().NotBeNull();
        }

        [Fact]
        public async void SignInAsync_SignInWithWrongPassword_ReturnsServiceResultWithBusinessError()
        {
            //Arrange
            CancellationToken token = default;
            var signInModel = _fixture.Create<SignInModel>();
            var identity = _fixture.Create<IdentityModel>();
            var password = _fixture.Create<string>();

            _identityRepositoryMock.Setup(x => x.GetByEmailOrUserTagAsync(It.IsAny<EmailOrUserTagSearchModel>(), token))
                .Returns(Task.FromResult(identity));

            _encryptorMock.Setup(x => x.EncryptPassword(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Returns(password);

            _encryptorMock.Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>()))
               .Returns(false);

            //Act
            var actual = await _signInService.SignInAsync(signInModel, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(ErrorType.BusinessError);
        }

        [Fact]
        public async void SignInAsync_SignInInexistentIdentity_ReturnsServiceResultWithBusinessError()
        {
            //Arrange
            CancellationToken token = default;
            var signInModel = _fixture.Create<SignInModel>();

            //Act
            var actual = await _signInService.SignInAsync(signInModel, token);

            //Assert
            actual.IsSuccessful.Should().BeFalse();
            actual.Error.Type.Should().Be(ErrorType.BusinessError);
        }
    }
}
