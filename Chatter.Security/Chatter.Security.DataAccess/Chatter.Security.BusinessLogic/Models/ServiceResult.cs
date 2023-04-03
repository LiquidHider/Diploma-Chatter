namespace Chatter.Security.Core.Models
{
    public class ServiceResult
    {
        public ErrorModel? Error { get; set; }

        public bool IsSuccessful => Error is null;
    }
}
