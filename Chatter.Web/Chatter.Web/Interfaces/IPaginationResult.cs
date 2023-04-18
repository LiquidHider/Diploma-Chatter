namespace Chatter.Web.Interfaces
{
    public interface IPaginationResult<TSort> : IPaginationParameters<TSort> where TSort : Enum
    {
        int TotalCount { get; }

        int TotalPages { get; }
    }
}
