using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.DataAccess.Models.Pagination;
using Chatter.Domain.DataAccess.Models.Parameters;

namespace Chatter.Domain.DataAccess.Interfaces
{
    public interface IReportRepository
    {
        Task<ReportModel> GetAsync(Guid id, CancellationToken cancellationToken);

        Task CreateAsync(ReportModel item, CancellationToken cancellationToken);

        Task<DeletionStatus> DeleteAsync(ReportModel item, CancellationToken cancellationToken);

        Task<PaginatedResult<ReportModel,ReportSort>> ListASync(ReportsListParameters listParameters, CancellationToken cancellationToken);
    }
}
