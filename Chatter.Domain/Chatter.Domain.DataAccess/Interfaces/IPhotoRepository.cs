using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.DataAccess.Models.Parameters;

namespace Chatter.Domain.DataAccess.Interfaces
{
    internal interface IPhotoRepository
    {
        Task<Photo> GetAsync(Guid id, CancellationToken cancellationToken);

        Task CreateAsync(Photo item, CancellationToken cancellationToken);

        Task<DeletionStatus> DeleteAsync(Photo item, CancellationToken cancellationToken);

        Task<IList<Photo>> ListAsync(PhotosListParameters listParameters, CancellationToken cancellationToken);
    }
}
