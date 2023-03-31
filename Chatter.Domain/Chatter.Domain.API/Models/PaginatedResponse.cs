using Chatter.Domain.Common.Enums;
using Chatter.Domain.Common.Interfaces;

namespace Chatter.Domain.API.Models
{
     public class PaginatedResponse<TModel, TSort> : IPaginationResult<TSort> where TSort : Enum
     {
         public int PageNumber { get; set; }
    
         public int PageSize { get; set; }
    
         public SortOrder SortOrder { get; set; }
    
         public TSort SortBy { get; set; }
    
         public int TotalCount { get; set; }
    
         public int TotalPages { get; set; }
    
         public IList<TModel>? Items { get; set; }
     }
}
