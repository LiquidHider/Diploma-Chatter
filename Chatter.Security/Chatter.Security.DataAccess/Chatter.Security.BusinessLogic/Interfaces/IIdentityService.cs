using Chatter.Security.Core.Models;

namespace Chatter.Security.Core.Interfaces
{
    public interface IIdentityService
    {
        Task<ValueServiceResult<Guid>> CreateAsync(CreateIdentity createModel, CancellationToken cancellationToken);

        Task<ValueServiceResult<Guid>> UpdateAsync(UpdateIdentity updateModel, CancellationToken cancellationToken);

        Task<ValueServiceResult<Guid>> DeleteAsync(Guid id, CancellationToken cancellationToken);

        Task<ValueServiceResult<Identity>> FindByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<ValueServiceResult<Identity>> FindByEmailAsync(string email, CancellationToken cancellationToken);

        Task<ValueServiceResult<Identity>> FindByUserTagAsync(string userTag, CancellationToken cancellationToken);

        Task<ValueServiceResult<IList<string>>> GetRolesAsync(Guid identityId, CancellationToken cancellationToken);
    }
}
