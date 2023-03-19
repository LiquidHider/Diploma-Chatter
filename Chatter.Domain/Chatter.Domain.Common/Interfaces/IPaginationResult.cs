using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Domain.Common.Interfaces
{
    public interface IPaginationResult<TSort> : IPaginationParameters<TSort> where TSort : Enum
    {
        int TotalCount { get; }

        int TotalPages { get; }
    }
}
