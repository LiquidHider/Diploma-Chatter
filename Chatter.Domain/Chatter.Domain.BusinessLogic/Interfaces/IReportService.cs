﻿using Chatter.Domain.BusinessLogic.Models;
using Chatter.Domain.BusinessLogic.Models.Create;

namespace Chatter.Domain.BusinessLogic.Interfaces
{
    public interface IReportService
    {
        Task<ValueServiceResult<Guid>> SendReportAsync(Report report, CancellationToken cancellationToken);

        Task<ValueServiceResult<Report>> CreateReportAsync(CreateReport createReportModel, CancellationToken cancellationToken);

        Task<ValueServiceResult<Guid>> RemoveReportAsync(Guid id, CancellationToken cancellationToken);
    }
}
