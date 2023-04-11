using AutoMapper;
using Chatter.Security.API.Interfaces;
using Chatter.Security.API.Models.Login;
using Chatter.Security.API.Models.Register;
using Chatter.Security.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Chatter.Security.API.Controllers
{
    public class AccountController : BaseAPIController
    {
        private readonly IAccountService _accountService;
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;

        public AccountController(IMapper mapper, IAccountService accountService, IIdentityService identityService,
            IEmailService emailService) : base(mapper)
        {
            _accountService = accountService;
            _emailService = emailService;
            _identityService = identityService;
        }


        [HttpPost]
        [Route("signIn")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SignInResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignInAsync(SignInRequest requestModel, CancellationToken cancellationToken)
        {
            var result = await _accountService.SignInAsync(requestModel, cancellationToken); 
            
            if (!result.IsSuccessful)
            {
                return MapErrorResponse(result);
            }

            return Ok(result.Value);
        }


        [HttpPost]
        [Route("signUp")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SignUpResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignUpAsync(SignUpRequest requestModel, CancellationToken cancellationToken)
        {
            var result = await _accountService.SignUpAsync(requestModel, cancellationToken);

            if (!result.IsSuccessful)
            {
                return MapErrorResponse(result);
            }

            _emailService.SendCongratulationsMessageToNewUser(requestModel.Email);

            return Ok(result.Value);
        }

        [HttpPost]
        [Route("exists")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> IsIdentityExistsAsync(string? email, string? userTag, CancellationToken cancellationToken)
        {
            var result = await _identityService.FindByEmailOrUserTagAsync(email, userTag, cancellationToken); 

            return Ok(!result.IsEmpty);
        }
    }
}
