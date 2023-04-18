namespace Chatter.Web.Interfaces
{
    public interface IPaginationParameters<TSort> : ISortingParameters<TSort> where TSort : Enum
    {
        int PageNumber { get; }

        int PageSize { get; }
    }
}
