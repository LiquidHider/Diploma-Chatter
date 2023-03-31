using Chatter.Domain.API.Models.ChatUser;
using FluentValidation;

namespace Chatter.Domain.API.Validations
{
    public class BlockUserRequestValidator : AbstractValidator<BlockUserRequest>
    {
        public BlockUserRequestValidator()
        {
            RuleFor(x => x.UserID).NotEqual(Guid.Empty).NotNull();
            RuleFor(x => x.BlockedUntilUtc).GreaterThan(DateTime.Now);
        }
    }
}
