using Chatter.Security.Common.Enums;
using Chatter.Security.DataAccess.Models;

namespace Chatter.Security.DataAccess.Interfaces
{
    public interface IIdentityRepository
    {
        Task CreateAsync(CreateIdentityModel createModel, CancellationToken cancellationToken);

        Task<bool> UpdateAsync(UpdateIdentityModel updateModel, CancellationToken cancellationToken);

        Task<IdentityModel> GetAsync(Guid id, CancellationToken cancellationToken);

        Task<DeletionStatus> DeleteAsync(Guid id, CancellationToken cancellationToken);

        Task<IdentityModel> GetByEmailOrUserTagAsync(GetByEmailOrUserTag searchModel, CancellationToken cancellationToken);
    }
}
