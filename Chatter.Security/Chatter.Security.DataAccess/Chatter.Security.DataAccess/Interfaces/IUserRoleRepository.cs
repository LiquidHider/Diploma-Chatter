using Chatter.Security.Common.Enums;

namespace Chatter.Security.DataAccess.Interfaces
{
    public interface IUserRoleRepository
    {
        Task<Guid> GetRoleIdAsync(Guid identityId, UserRole userRole, CancellationToken cancellationToken);

        Task<IList<UserRole>> GetUserRolesAsync(Guid identityId, CancellationToken cancellationToken);

        Task<Guid> AddRoleToUserAsync(Guid identityId, UserRole userRole, CancellationToken cancellationToken);

        Task<DeletionStatus> DeleteUserRoleAsync(Guid identityId, UserRole userRole, CancellationToken cancellationToken);
    }
}
