using Chatter.Domain.Common.Enums;

namespace Chatter.Domain.DataAccess.Models.Parameters
{
    public class ReportsListParameters
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public SortOrder SortOrder { get; set; }

        public ReportSort SortBy { get; set; }

        public IList<Guid>? ReportedUsersIDs { get; set; }

        public string? Search { get; set; }
    }
}
