using Chatter.Domain.Common.Enums;
using Chatter.Domain.Common.Interfaces;

namespace Chatter.Domain.BusinessLogic.Models
{
    public class PaginatedResult<TModel, TSort> : ValueServiceResult<IList<TModel>>, IPaginationResult<TSort> where TSort : Enum
    {
        public PaginatedResult() {}

        public PaginatedResult(IPaginationResult<TSort> pagination)
        {
            if (pagination is null)
            {
                throw new ArgumentNullException(nameof(pagination));
            }

            PageNumber = pagination.PageNumber;
            PageSize = pagination.PageSize;
            SortBy = pagination.SortBy;
            SortOrder = pagination.SortOrder;
            TotalCount = pagination.TotalCount;
            TotalPages = pagination.TotalPages;
        }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public SortOrder SortOrder { get; set; }

        public TSort SortBy { get; set; }

        public override bool IsEmpty => Value is null || Value.Count == 0;
    }
}
