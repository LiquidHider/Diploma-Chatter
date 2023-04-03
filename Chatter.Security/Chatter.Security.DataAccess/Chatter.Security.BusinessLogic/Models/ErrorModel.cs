using Chatter.Security.Core.Enums;

namespace Chatter.Security.Core.Models
{
    public class ErrorModel
    {
        public ErrorType Type { get; set; }

        public string? Message { get; set; }
    }
}
