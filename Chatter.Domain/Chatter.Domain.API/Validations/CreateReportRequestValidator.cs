using Chatter.Domain.API.Models.Reports;
using FluentValidation;

namespace Chatter.Domain.API.Validations
{
    public class CreateReportRequestValidator : AbstractValidator<CreateReportRequest>
    {
        public CreateReportRequestValidator() 
        {
            RuleFor(x => x.ReportedUserID).NotEqual(Guid.Empty).NotNull();
            RuleFor(x => x.Title).Length(2, 20).NotNull();
            RuleFor(x => x.Message).Length(2, 500).NotNull();
        }
    }
}
