using Chatter.Domain.API.Models.PrivateChat;
using FluentValidation;

namespace Chatter.Domain.API.Validations
{
    public class CreateChatMessageRequestValidator : AbstractValidator<CreateChatMessageRequest>
    {
        public CreateChatMessageRequestValidator()
        {
            RuleFor(x => x.SenderID).NotEqual(Guid.Empty).NotNull();
            RuleFor(x => x.RecipientID).NotEqual(Guid.Empty).NotNull();
            RuleFor(x => x.Body).Length(1, 4000).NotNull();
        }
    }
}
