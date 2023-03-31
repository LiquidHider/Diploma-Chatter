using Chatter.Domain.Common.Enums;

namespace Chatter.Domain.API.Models
{
    public abstract class PaginationRequestBase<TSort> where TSort : Enum
    {
        public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
            public SortOrder? SortOrder { get; set; }
            public TSort? SortBy { get; set; }
    }
}
