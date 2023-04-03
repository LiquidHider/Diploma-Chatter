using Chatter.Security.Core.Interfaces;
using Chatter.Security.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Chatter.Security.Core.Services
{
    public class TokenService : ITokenService
    {
        private const int TOKEN_EXPIRATION_DATE_HOURS = 7;
        private readonly SymmetricSecurityKey key;
        private readonly IIdentityService _identityService;

        public TokenService(IConfiguration config, IIdentityService identityService)
        {
            key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            _identityService = identityService;
        }
        public async Task<string> CreateTokenAsync(Identity identity, CancellationToken cancellationToken)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.NameId, identity.ID.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, identity.UserTag),
            };

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
