using Chatter.Domain.API.Models.Reports;
using FluentValidation;

namespace Chatter.Domain.API.Validations
{
    public class ReportsListRequestValidator : AbstractValidator<ReportsListRequest>
    {
        public ReportsListRequestValidator()
        {
            RuleFor(x => x.PageSize).GreaterThan(0).NotNull();
            RuleFor(x => x.PageNumber).GreaterThan(0).NotNull();
            RuleFor(x => (int)x.SortOrder).GreaterThan(0).NotNull();
            RuleFor(x => (int)x.SortBy).GreaterThan(0).NotNull();
            RuleFor(x => x.ReportedUsersIDs).Must(x => x.All(y => y != Guid.Empty)).WithMessage("Required reported users cant be with id: 00000000-0000-0000-0000-000000000000.");
        }
    }
}
