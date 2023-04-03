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
        private readonly ILogger<SignInService> _logger;
        public SignInService(IIdentityRepository identityRepository, ILogger<SignInService> logger)
        {
            _identityRepository = identityRepository;
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

            bool isPasswordValid = VerifyPassword
            (
               password: user.PasswordHash,
               storedPasswordHash: user.PasswordHash,
               key: Encoding.UTF8.GetBytes(user.PasswordKey)
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
                ClaimsIdentity identity = new ClaimsIdentity(claims, "AuthCookie");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                return result.WithValue(claimsPrincipal);
           
        }

        private bool VerifyPassword(string password, string storedPasswordHash, byte[] key)
        {
            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                byte[] computedPasswordHashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                string computedPasswordHashString = Convert.ToBase64String(computedPasswordHashBytes);
                byte[] computedPasswordHashStringBytes = Encoding.UTF8.GetBytes(computedPasswordHashString);
                byte[] storedPasswordHashBytes = Encoding.UTF8.GetBytes(storedPasswordHash);

                if (storedPasswordHashBytes.Length != computedPasswordHashStringBytes.Length)
                {
                    return false;
                }

                for (int i = 0; i < storedPasswordHashBytes.Length; i++)
                {
                    if (storedPasswordHashBytes[i] != computedPasswordHashStringBytes[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
