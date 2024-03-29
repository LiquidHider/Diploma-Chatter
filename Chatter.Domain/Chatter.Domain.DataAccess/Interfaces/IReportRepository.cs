﻿using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.Models;
using Chatter.Domain.DataAccess.Models.Pagination;
using Chatter.Domain.DataAccess.Models.Parameters;

namespace Chatter.Domain.DataAccess.Interfaces
{
    public interface IReportRepository
    {
        Task<ReportModel> GetAsync(Guid id, CancellationToken cancellationToken);

        Task CreateAsync(ReportModel item, CancellationToken cancellationToken);

        Task<DeletionStatus> DeleteAsync(Guid id, CancellationToken cancellationToken);

        Task<PaginatedResult<ReportModel,ReportSort>> ListAsync(ReportsListParameters listParameters, CancellationToken cancellationToken);
    }
}
