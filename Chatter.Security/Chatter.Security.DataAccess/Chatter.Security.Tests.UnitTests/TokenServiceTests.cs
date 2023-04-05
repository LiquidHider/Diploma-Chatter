using AutoFixture;
using Chatter.Security.Common;
using Chatter.Security.Common.Extensions;
using Chatter.Security.Core.Interfaces;
using Chatter.Security.Core.Models;
using Chatter.Security.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Chatter.Security.Tests.UnitTests
{
    public class TokenServiceTests
    {
        private readonly TokenService _tokenService;
        private readonly Fixture _fixture;
        private readonly Mock<ILogger<TokenService>> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IIdentityService> _identityServiceMock;

        public TokenServiceTests()
        {
            _fixture = new Fixture();

            _loggerMock = new Mock<ILogger<TokenService>>();
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(x => x["TokenKey"]).Returns(_fixture.Create<string>());
            _identityServiceMock = new Mock<IIdentityService>();

            _tokenService = new TokenService(_configurationMock.Object, _identityServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async void CreateTokenAsync_CreateTokenForExistingIdentity_ReturnsToken() 
        {
            //Arrange
            CancellationToken token = default;
            var identity = _fixture.Create<Identity>();
            var valueServiceResultIdentity = new ValueServiceResult<Identity>().WithValue(identity);
           
            _identityServiceMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), token))
                .Returns(Task.FromResult(valueServiceResultIdentity));
            var userRoles = _fixture.Create<IList<string>>();

            var valueServiceResultUserRoles = new ValueServiceResult<IList<string>>().WithValue(userRoles);
            _identityServiceMock.Setup(x => x.GetRolesAsync(It.IsAny<Guid>(), token))
                .Returns(Task.FromResult(valueServiceResultUserRoles));

            //Act
            var actual = await _tokenService.CreateTokenAsync(identity, token);

            //Assert
            actual.Should().NotBeEmpty();

        }

        [Fact]
        public async void CreateTokenAsync_CreateTokenForInexistentIdentity_ReturnsEmptyString()
        {
            //Arrange
            CancellationToken token = default;
            var identity = _fixture.Create<Identity>();

            //Act
            var actual = await _tokenService.CreateTokenAsync(identity, token);

            //Assert
            actual.Should().BeEmpty();
        }
    }
}
