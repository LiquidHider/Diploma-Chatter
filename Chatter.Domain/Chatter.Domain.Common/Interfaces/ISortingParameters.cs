using Chatter.Domain.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Domain.Common.Interfaces
{
    public interface ISortingParameters<TSort> where TSort : Enum
    {
        SortOrder SortOrder { get; }

        TSort SortBy { get; }
    }
}
