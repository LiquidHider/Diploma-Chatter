using AutoMapper;
using Chatter.Security.API.Interfaces;
using Chatter.Security.API.Models.Login;
using Chatter.Security.API.Models.Register;
using Chatter.Security.Common;
using Chatter.Security.Common.Enums;
using Chatter.Security.Common.Extensions;
using Chatter.Security.Core.Interfaces;
using Chatter.Security.Core.Models;

namespace Chatter.Security.API.Services
{
    public class AccountService : IAccountService
    {
        private readonly IIdentityService _identityService;
        private readonly ISignInService _signInService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AccountService> _logger;
        private readonly IMapper _mapper;

        public AccountService(IIdentityService identityService, ITokenService tokenService,
            IMapper mapper, ISignInService signInService, ILogger<AccountService> logger)
        {
            _identityService = identityService;
            _tokenService = tokenService;
            _mapper = mapper;
            _signInService = signInService;
            _logger = logger;
        }

        public async Task<ValueServiceResult<SignInResponse>> SignInAsync(SignInRequest signInRequest, CancellationToken cancellationToken)
        {
            var result = new ValueServiceResult<SignInResponse>();
            _logger.LogInformation("SignInAsync : {@Details}", new { Class = nameof(AccountService), Method = nameof(SignInAsync) });
            try
            {
                var identity = await _identityService.FindByEmailOrUserTagAsync(signInRequest.Email, signInRequest.UserTag, cancellationToken);

                if (identity == null) 
                {
                    _logger.LogInformation("Identity does not exist. {@Details}", new { UserTag = signInRequest.UserTag, Email = signInRequest.Email });
                    return result.WithBusinessError("Identity does not exist.");
                }
                var mappedRequest = _mapper.Map<SignInModel>(signInRequest);
                var claimsPrincipal = await _signInService.SignInAsync(mappedRequest, cancellationToken);

                if (!claimsPrincipal.IsSuccessful) 
                {
                    return result.WithBusinessError(claimsPrincipal.Error.Message);
                }

                var response = new SignInResponse()
                {
                    UserID = identity.Value.UserID,
                    Token = await _tokenService.CreateTokenAsync(identity.Value, cancellationToken)
                };

                return result.WithValue(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }

        public async Task<ValueServiceResult<SignUpResponse>> SignUpAsync(SignUpRequest signUpRequest, CancellationToken cancellationToken)
        {
            var result = new ValueServiceResult<SignUpResponse>();
            try
            {
                var identity = await _identityService.FindByEmailOrUserTagAsync(signUpRequest.Email, signUpRequest.UserTag, cancellationToken);

                if (identity.Value != null) 
                {
                    _logger.LogInformation("Identity already exists. {@Details}", new { UserTag = signUpRequest.UserTag, Email = signUpRequest.Email });
                    return result.WithBusinessError("Identity already exists.");
                }

                var createModel = new CreateIdentity()
                {
                    Email = signUpRequest.Email,
                    UserTag = signUpRequest.UserTag,
                    Password = signUpRequest.Password,
                    UserID = signUpRequest.UserId,
                };
                createModel.Roles.Add(UserRole.DefaultUser);

                await _identityService.CreateAsync(createModel, cancellationToken);
                var createdIdentity = await _identityService.FindByEmailOrUserTagAsync(signUpRequest.Email, signUpRequest.UserTag, cancellationToken);
                var response = new SignUpResponse()
                {
                    UserID = signUpRequest.UserId,
                    Token = await _tokenService.CreateTokenAsync(createdIdentity.Value, cancellationToken)
                };

                return result.WithValue(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return result.WithException(ex.Message);
            }
        }
    }
}
