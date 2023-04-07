using Chatter.Email.Common.Enums;

namespace Chatter.Email.Common.ServiceResults
{
    public class ErrorModel
    {
        public ErrorType Type { get; set; }

        public string? Message { get; set; }
    }
}
