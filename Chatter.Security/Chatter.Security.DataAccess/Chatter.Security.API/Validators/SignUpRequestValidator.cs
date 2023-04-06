using Chatter.Security.API.Models.Register;
using FluentValidation;

namespace Chatter.Security.API.Validators
{
    public class SignUpRequestValidator : AbstractValidator<SignUpRequest>
    {
        public SignUpRequestValidator()
        {
            RuleFor(x => x.Email)
                .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
                .WithMessage("Wrong email format");
            RuleFor(x => x.UserTag).Length(2, 20);
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
