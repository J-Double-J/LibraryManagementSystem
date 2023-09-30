using Domain.CustomFluentValidation;
using FluentValidation;

namespace Infrastructure.Configuration.Objects
{
    internal class RenewalPolicyValidator : LibraryValidator<RenewalPolicy>
    {
        public RenewalPolicyValidator()
            : base(LibraryValidatorType.Configuration)
        {
            RuleFor(policy => policy.MaximumRenewals).GreaterThanOrEqualTo(0);
            RuleFor(policy => policy.RenewalPeriodInDays).GreaterThanOrEqualTo(1);
        }
    }
}
