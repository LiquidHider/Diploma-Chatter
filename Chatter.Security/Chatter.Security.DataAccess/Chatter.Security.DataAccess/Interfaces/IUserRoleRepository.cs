using Chatter.Security.Common.Enums;

namespace Chatter.Security.DataAccess.Interfaces
{
    public interface IUserRoleRepository
    {
        Task<UserRole> GetRoleIdAsync(Guid userId, UserRole userRole, CancellationToken cancellationToken);

        Task<IList<UserRole>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken);

        Task AddRoleToUserAsync(Guid userId, UserRole userRole, CancellationToken cancellationToken);

        Task<DeletionStatus> DeleteUserRoleAsync(Guid userId, UserRole userRole, CancellationToken cancellationToken);
    }
}
