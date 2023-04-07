namespace Chatter.Email.Common.ServiceResults
{
    public class ServiceResult
    {
        public ErrorModel? Error { get; set; }

        public bool IsSuccessful => Error is null;
    }
}
