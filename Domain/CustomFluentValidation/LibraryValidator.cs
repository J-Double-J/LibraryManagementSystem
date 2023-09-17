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
        /// <summary>
        /// Constructs the LibraryValidator to be a wrapper around <see cref="AbstractValidator{T}"/>.
        /// </summary>
        /// <param name="validatorType">Type of validation this validator is responsible for. This help determines the default for part of the validation
        /// error code.</param>
        /// <param name="isSlowValidator">Sets whether the validator would be considered slow. See <see cref="IsSlowValidator"/>.</param>
        public LibraryValidator(LibraryValidatorType validatorType, bool isSlowValidator = false)
        {
            LibraryValidatorType = validatorType;
            IsSlowValidator = isSlowValidator;
        }

        /// <summary>
        /// Gets the library validator level.
        /// </summary>
        public LibraryValidatorType LibraryValidatorType { get; init; }

        /// <summary>
        /// Gets or sets if the validator would be considered 'slow'.
        /// </summary>
        /// <remarks>
        /// If a validator must wait on an IO/Database/HTTP request to validate something,
        /// then it is considered slow. These validators should be run AFTER all fast validators successfully finish and are valid.
        /// </remarks>
        public bool IsSlowValidator { get; init; }

        public override async Task<ValidationResult> ValidateAsync(ValidationContext<T> context, CancellationToken cancellation = default)
        {
            ValidationResult validationResult = await base.ValidateAsync(context, cancellation);

            return validationResult.ToLibraryFluentValidationResult(LibraryValidatorType);
        }

        public async Task<DomainValidationResult<T>> ValidateAsyncGetDomainResult(T instance, CancellationToken cancellation = default)
        {
            ValidationResult validationResult = await ValidateAsync(instance, cancellation);

            return validationResult.ToDomainValidationResult(instance, LibraryValidatorType);
        }
    }
}
