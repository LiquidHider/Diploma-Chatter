using Chatter.Domain.Common.Enums;

namespace Chatter.Domain.API.Models.Reports
{
    public class ReportsListRequest : PaginationRequestBase<ReportSort>
    {
        public IList<Guid>? ReportedUsersIDs { get; set; }
    }
}
