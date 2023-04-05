using Chatter.Security.API.Models.Login;
using Chatter.Security.API.Models.Register;
using Chatter.Security.Common;

namespace Chatter.Security.API.Interfaces
{
    public interface IAccountService
    {
        Task<ValueServiceResult<SignInResponse>> SignInAsync(SignInRequest loginRequest, CancellationToken cancellationToken);

        Task<ValueServiceResult<SignUpResponse>> SignUpAsync(SignUpRequest registerRequest, CancellationToken cancellationToken);
    }
}
