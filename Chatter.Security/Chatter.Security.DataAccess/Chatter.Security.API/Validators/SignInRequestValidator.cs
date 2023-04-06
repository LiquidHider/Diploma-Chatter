using Chatter.Security.API.Models.Login;
using FluentValidation;

namespace Chatter.Security.API.Validators
{
    public class SignInRequestValidator : AbstractValidator<SignInRequest>
    {
        public SignInRequestValidator()
        {
            RuleFor(x => x.Email).MinimumLength(1);
            RuleFor(x => x.UserTag).MinimumLength(1);
            RuleFor(x => x.Password).NotNull().NotEmpty();
        }
    }
}
