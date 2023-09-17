using Domain.Abstract;
using FluentValidation.Results;

namespace Domain.CustomFluentValidation
{
    public static class FluentValidationExtensions
    {
        public static LibraryFluentValidationResult ToLibraryFluentValidationResult(this ValidationResult validationResult, LibraryValidatorType validatorLevel)
        {
            return new LibraryFluentValidationResult()
            {
                ValidationLevel = validatorLevel,

                // Error codes will be overriden at the rule level in case of mixed.
                Errors = validatorLevel != LibraryValidatorType.Mixed
                            ? PrefixErrorCodesToErrors(validationResult, validatorLevel)
                            : validationResult.Errors
            };
        }

        public static DomainValidationResult ToDomainValidationResult(this ValidationResult validationResult, LibraryValidatorType type)
        {
            LibraryFluentValidationResult libFluentResult = validationResult.ToLibraryFluentValidationResult(type);

            if (libFluentResult.IsValid)
            {
                return DomainValidationResult.SuccessfulValidation();
            }

            ValidationError[] errors = MapValidationFailuresToValidationDomainErrors(libFluentResult);

            return DomainValidationResult.WithErrors(errors);
        }

        public static DomainValidationResult<TValue> ToDomainValidationResult<TValue>(this ValidationResult validationResult,
                                                                                        TValue value,
                                                                                        LibraryValidatorType type)
        {
            LibraryFluentValidationResult libFluentResult = validationResult.ToLibraryFluentValidationResult(type);

            if (libFluentResult.IsValid)
            {
                return DomainValidationResult<TValue>.SuccessfulValidation(value);
            }

            ValidationError[] errors = MapValidationFailuresToValidationDomainErrors(libFluentResult);

            return DomainValidationResult<TValue>.WithErrors(errors);
        }

        private static List<ValidationFailure> PrefixErrorCodesToErrors(ValidationResult validationResult, LibraryValidatorType validatorLevel)
        {
            List<ValidationFailure> failures = validationResult.Errors;
            
            foreach(var error in failures)
            {
                error.ErrorCode = LibraryValidationErrorCodeHelper.PrefixErrorCodeIfNoPrefixAttached(validatorLevel, error.ErrorCode);
            }

            return failures;
        }

        private static ValidationError[] MapValidationFailuresToValidationDomainErrors(LibraryFluentValidationResult libFluentResult)
        {
            ValidationError[] errors = new ValidationError[libFluentResult.Errors.Count];

            for (int i = 0; i < libFluentResult.Errors.Count; i++)
            {
                var error = libFluentResult.Errors[i];
                errors[i] = new(error.ErrorCode, "Validation Failure:", error.ErrorMessage);
            }

            return errors;
        }
    }
}
