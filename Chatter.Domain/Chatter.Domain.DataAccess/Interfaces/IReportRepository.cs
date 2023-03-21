using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.DataAccess.Models.Pagination;
using Chatter.Domain.DataAccess.Models.Parameters;

namespace Chatter.Domain.DataAccess.Interfaces
{
    public interface IReportRepository
    {
        Task<Report> GetAsync(Guid id, CancellationToken cancellationToken);

        Task CreateAsync(Report item, CancellationToken cancellationToken);

        Task<DeletionStatus> DeleteAsync(Report item, CancellationToken cancellationToken);

        Task<PaginatedResult<Report,ReportSort>> ListASync(ReportsListParameters listParameters, CancellationToken cancellationToken);
    }
}
