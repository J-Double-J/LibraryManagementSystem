using Domain.Abstract;
using FluentValidation;
using FluentValidation.Results;

namespace Domain.CustomFluentValidation
{
    /// <summary>
    /// This class is a custom wrapper around Abstract Validator that will map <see cref="Library.DomainValidationResult"/>
    /// to a <see cref="LibraryFluentValidationResult"/> after validating async.
    /// </summary>
    /// <remarks>
    /// This is done because in some cases there is an internal error that could occur on validation. In order to mask internals,
    /// it is easier to check the type of the validator that failed so return messages can be mapped appropriately.
    ///</remarks>
    /// <typeparam name="T">Type of object to validate.</typeparam>
    public class LibraryValidator<T> : AbstractValidator<T>
    {
        public LibraryValidator(LibraryValidatorType validatorType)
        {
            LibraryValidatorType = validatorType;
        }

        /// <summary>
        /// Gets the library validator level.
        /// </summary>
        public LibraryValidatorType LibraryValidatorType { get; init; }

        public override async Task<ValidationResult> ValidateAsync(ValidationContext<T> context, CancellationToken cancellation = default)
        {
            ValidationResult validationResult = await base.ValidateAsync(context, cancellation);

            return validationResult.ToLibraryFluentValidationResult(LibraryValidatorType);
        }

        public async Task<DomainValidationResult> ValidateAsyncGetDomainResult(T instance, CancellationToken cancellation = default)
        {
            ValidationResult validationResult = await ValidateAsync(instance, cancellation);

            if(validationResult.IsValid)
            {
                return DomainValidationResult.SuccessfulValidation();
            }

            return validationResult.ToDomainValidationResult(LibraryValidatorType);
        }
    }
}
