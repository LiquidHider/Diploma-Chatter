using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.DataAccess.Models.Pagination;
using Chatter.Domain.DataAccess.Models.Parameters;

namespace Chatter.Domain.DataAccess.Interfaces
{
    public interface IPhotoRepository
    {
        Task<PhotoModel> GetAsync(Guid id, CancellationToken cancellationToken);

        Task CreateAsync(PhotoModel item, CancellationToken cancellationToken);

        Task<DeletionStatus> DeleteAsync(Guid id, CancellationToken cancellationToken);

        Task<PaginatedResult<PhotoModel, PhotoSort>> ListAsync(PhotosListParameters listParameters, CancellationToken cancellationToken);
    }
}
