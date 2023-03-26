using Chatter.Domain.BusinessLogic.Models;
using Chatter.Domain.BusinessLogic.Models.Create;

namespace Chatter.Domain.BusinessLogic.Interfaces
{
    public interface IReportService
    {
        Task<ValueServiceResult<Guid>> SendReport(Report report, CancellationToken cancellationToken);

        Task<ValueServiceResult<Report>> CreateReport(CreateReport createReportModel, CancellationToken cancellationToken);

        Task<ValueServiceResult<Guid>> RemoveReport(Guid id, CancellationToken cancellationToken);
    }
}
