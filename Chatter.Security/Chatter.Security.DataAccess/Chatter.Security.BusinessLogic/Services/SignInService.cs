using Chatter.Security.Core.Extensions;
using Chatter.Security.Core.Interfaces;
using Chatter.Security.Core.Models;
using Chatter.Security.DataAccess.Interfaces;
using Chatter.Security.DataAccess.Models;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Chatter.Security.Core.Services
{
    public class SignInService : ISignInService
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IHMACEncryptor _encryptor;
        private readonly ILogger<SignInService> _logger;
        public SignInService(IIdentityRepository identityRepository, IHMACEncryptor encryptor, ILogger<SignInService> logger)
        {
            _identityRepository = identityRepository;
            _encryptor = encryptor;
            _logger = logger;
        }

        public async Task<ValueServiceResult<ClaimsPrincipal>> SignInAsync(SignInModel signInModel, CancellationToken cancellationToken)
        {
            _logger.LogInformation("SignInAsync : {@Details}", new { Class = nameof(SignInService), Method = nameof(SignInAsync) });
            var result = new ValueServiceResult<ClaimsPrincipal>();

            var searchModel = new EmailOrUserTagSearchModel() 
            {
                Email = signInModel.Email,
                UserTag = signInModel.UserTag
            };
            var user = await _identityRepository.GetByEmailOrUserTagAsync(searchModel, cancellationToken);

            if (user == null)
            {
                _logger.LogInformation("User does not exist. : {@Details}", new { Email = signInModel.Email, UserTag = signInModel.UserTag });
                return result.WithBusinessError("User does not exist.");
            }

            var passwordKey = Encoding.UTF8.GetBytes(user.PasswordKey);
            var encryptedPassword = _encryptor.EncryptPassword(signInModel.Password, passwordKey);

            bool isPasswordValid = _encryptor.Verify
            (
               decryptedValue: encryptedPassword,
               encryptedValue: user.PasswordHash,
               key: passwordKey
            );

            if (!isPasswordValid)
            {
                _logger.LogInformation("Password is not valid. : {@Details}", new { Email = signInModel.Email, UserTag = signInModel.UserTag });
                return result.WithBusinessError("Password is not valid.");
            }
            
            List<Claim> claims = new List<Claim> ();

            if (signInModel.UserTag != null) 
            {
                claims.Add(new Claim(ClaimTypes.Name, user.UserTag));
            }
            if (signInModel.Email != null)
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
            }

            var authType = "AuthCookie";
            ClaimsIdentity identity = new ClaimsIdentity(claims, authType);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

            return result.WithValue(claimsPrincipal);
           
        }
    }
}
