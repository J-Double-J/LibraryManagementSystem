using Application.Interfaces.Configuration;
using Domain.CustomFluentValidation;
using FluentValidation.Results;

namespace Infrastructure.Configuration.Objects
{
    public class RenewalPolicy : IRenewalPolicy
    {
        public int MaximumRenewals { get; init; }
        public int RenewalPeriodInDays { get; init; }

        public LibraryFluentValidationResult Validate()
        {
            RenewalPolicyValidator validator = new();

            ValidationResult validationResult = validator.Validate(this);

            return new LibraryFluentValidationResult(validationResult, validator.LibraryValidatorType);
        }
    }
}
