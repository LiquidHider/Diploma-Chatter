using Chatter.Security.Core.Interfaces;
using Chatter.Security.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Chatter.Security.Core.Services
{
    public class TokenService : ITokenService
    {
        private const int TOKEN_EXPIRATION_DATE_HOURS = 7;
        private readonly ILogger<TokenService> _logger;
        private readonly IIdentityService _identityService;
        private readonly IConfiguration _configuration;

        public TokenService(IIdentityService identityService, IConfiguration configuration, ILogger<TokenService> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _identityService = identityService;
        }

        public async Task<string> CreateTokenAsync(Identity identity, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateTokenAsync : {@Details}", new { Class = nameof(TokenService), Method = nameof(CreateTokenAsync) });

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.NameId, identity.UserID.ToString()),
            };

            var identityFromDb = await _identityService.FindByIdAsync(identity.ID, cancellationToken);

            if (identityFromDb == null) 
            {
                _logger.LogInformation("Identity doest not exist. {@Details}", new { IdentityID = identity.ID });
                return string.Empty;
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtTokenKey"]));
            var serviceResult = await _identityService.GetRolesAsync(identity.ID, cancellationToken);

            var roles = serviceResult.Value;

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescryptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(TOKEN_EXPIRATION_DATE_HOURS),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescryptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
