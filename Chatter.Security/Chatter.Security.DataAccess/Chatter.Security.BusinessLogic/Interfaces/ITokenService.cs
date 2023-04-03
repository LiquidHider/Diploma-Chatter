using Chatter.Security.Core.Models;

namespace Chatter.Security.Core.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(Identity identity, CancellationToken cancellationToken); 
    }
}
