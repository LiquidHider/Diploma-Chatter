using Chatter.Domain.BusinessLogic.Enums;

namespace Chatter.Domain.BusinessLogic.Models
{
    public class ErrorModel
    {
        public ErrorType Type { get; set; }

        public string? Message { get; set; }
    }
}
