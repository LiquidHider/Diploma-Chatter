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
            RuleFor(x => x.Sent).LessThan(DateTime.Now).GreaterThan(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).NotNull();
            RuleFor(x => x.Body).Length(1, 4000).NotNull();
        }
    }
}
