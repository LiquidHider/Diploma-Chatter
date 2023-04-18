using Chatter.Web.Enums;

namespace Chatter.Web.Interfaces
{
    public interface ISortingParameters<TSort> where TSort : Enum
    {
        SortOrder SortOrder { get; }

        TSort SortBy { get; }
    }
}
