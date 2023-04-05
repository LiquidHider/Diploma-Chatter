namespace Chatter.Security.Common
{
    public class ServiceResult
    {
        public ErrorModel? Error { get; set; }

        public bool IsSuccessful => Error is null;
    }
}
