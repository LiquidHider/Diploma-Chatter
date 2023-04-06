using AutoMapper;
using Chatter.Security.API.Interfaces;
using Chatter.Security.API.Models.Login;
using Chatter.Security.API.Models.Register;
using Microsoft.AspNetCore.Mvc;

namespace Chatter.Security.API.Controllers
{
    public class AccountController : BaseAPIController
    {
        private readonly IAccountService _accountService;

        public AccountController(IMapper mapper, IAccountService accountService) : base(mapper)
        {
            _accountService = accountService;
        }


        [HttpPost]
        [Route("signIn")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SignInResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SignInAsync(SignInRequest requestModel, CancellationToken cancellationToken)
        {
            var result = await _accountService.SignInAsync(requestModel, cancellationToken); 
            
            if (!result.IsSuccessful)
            {
                return Unauthorized(result.Error.Message);
            }

            return Ok(result.Value);
        }


        [HttpPost]
        [Route("signUp")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SignInResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SignUpAsync(SignUpRequest requestModel, CancellationToken cancellationToken)
        {
            var result = await _accountService.SignUpAsync(requestModel, cancellationToken);

            if (!result.IsSuccessful)
            {
                return Unauthorized(result.Error.Message);
            }

            return Ok(result.Value);
        }
    }
}
