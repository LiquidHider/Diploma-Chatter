using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Domain.Common.Interfaces
{
    public interface IPaginationParameters<TSort> : ISortingParameters<TSort> where TSort : Enum
    {
        int PageNumber { get; }

        int PageSize { get; }
    }
}
