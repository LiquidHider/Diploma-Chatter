using Chatter.Security.Common;
using Chatter.Security.Core.Models;
using System.Security.Claims;

namespace Chatter.Security.Core.Interfaces
{
    public interface ISignInService
    {
        Task<ValueServiceResult<ClaimsPrincipal>> SignInAsync(SignInModel signInModel, CancellationToken cancellationToken);
    }
}
