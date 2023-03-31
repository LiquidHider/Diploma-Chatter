using Chatter.Domain.API.Models.ChatUser;
using FluentValidation;

namespace Chatter.Domain.API.Validations
{
    public class UpdateChatUserRequestValidator : AbstractValidator<UpdateChatUserRequest>
    {
        public UpdateChatUserRequestValidator() 
        {
            RuleFor(x => x.UserID).NotEqual(Guid.Empty).NotNull();
            RuleFor(x => x.LastName)
                 .Must(x => !x.Any(char.IsDigit)).WithMessage("Last name cannot contain digits.")
                 .Length(2, 20).NotNull();

            RuleFor(x => x.FirstName)
                .Must(x => !x.Any(char.IsDigit)).WithMessage("First name cannot contain digits.")
                .Length(2, 20).NotNull();

            RuleFor(x => x.Patronymic)
                .Must(x => !x.Any(char.IsDigit)).WithMessage("Patronymic cannot contain digits.")
                .Length(2, 20);

            RuleFor(x => x.UniversityName)
                .Must(x => !x.Any(char.IsDigit)).WithMessage("University name cannot contain digits.")
                .Length(2, 50);

            RuleFor(x => x.UniversityFaculty)
                .Must(x => !x.Any(char.IsDigit))
                .WithMessage("University faculty cannot contain digits.").Length(2, 50);
        }
    }
}
