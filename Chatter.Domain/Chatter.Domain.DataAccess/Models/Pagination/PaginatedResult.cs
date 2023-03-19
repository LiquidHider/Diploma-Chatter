using Chatter.Domain.Common.Enums;
using Chatter.Domain.Common.Interfaces;

namespace Chatter.Domain.DataAccess.Models.Pagination
{
    public class PaginatedResult<TEntity, TSort> : IPaginationResult<TSort> where TSort : Enum
    {
        public int TotalCount { get; set; }

        public int TotalPages { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public SortOrder SortOrder { get; set; }

        public TSort SortBy { get; set; }

        public IList<TEntity>? Entities { get; set; }
    }
}
