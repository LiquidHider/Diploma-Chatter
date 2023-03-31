using Chatter.Domain.API.Models.PrivateChat;
using FluentValidation;

namespace Chatter.Domain.API.Validations
{
    public class OpenPrivateChatRequestValidator : AbstractValidator<OpenPrivateChatRequest>
    {
        public OpenPrivateChatRequestValidator()
        {
            RuleFor(x => x.Member1ID).NotEqual(Guid.Empty).NotNull();
            RuleFor(x => x.Member2ID).NotEqual(Guid.Empty).NotNull();
        }
    }
}
