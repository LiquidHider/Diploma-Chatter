using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.Models;

namespace Chatter.Domain.DataAccess.Interfaces
{
    internal interface IReportRepository
    {
        Task<Report> GetAsync(Guid id, CancellationToken cancellationToken);

        Task CreateAsync(Report item, CancellationToken cancellationToken);

        Task UpdateAsync(Report item, CancellationToken cancellationToken);

        Task<DeletionStatus> DeleteAsync(Report item, CancellationToken cancellationToken);

        Task<IList<Report>> GetAllASync(CancellationToken cancellationToken);
    }
}
