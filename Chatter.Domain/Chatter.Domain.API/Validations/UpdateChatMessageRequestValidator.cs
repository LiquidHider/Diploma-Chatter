using Chatter.Domain.API.Models.PrivateChat;
using FluentValidation;

namespace Chatter.Domain.API.Validations
{
    public class UpdateChatMessageRequestValidator : AbstractValidator<UpdateChatMessageRequest>
    {
        public UpdateChatMessageRequestValidator()
        {
            RuleFor(x => x.ID).NotEqual(Guid.Empty).NotNull();
            RuleFor(x => x.Body).Length(5, 500).NotNull();
        }
    }
}
