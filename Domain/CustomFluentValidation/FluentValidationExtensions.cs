using Domain.Abstract;
using FluentValidation.Results;

namespace Domain.CustomFluentValidation
{
    public static class FluentValidationExtensions
    {
        public static LibraryFluentValidationResult ToLibraryFluentValidationResult(this ValidationResult validationResult, LibraryValidatorType libraryValidatorType)
        {
            return new LibraryFluentValidationResult()
            {
                LibraryValidatorType = libraryValidatorType,

                // Error codes will be overriden at the rule level in case of mixed.
                Errors = libraryValidatorType != LibraryValidatorType.Mixed
                            ? PrefixErrorCodesToErrors(validationResult, libraryValidatorType)
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
                error.ErrorCode = ValidationErrorCodeFactory.PrefixErrorCodeIfNoPrefixAttached(validatorLevel, error.ErrorCode).ToString();
            }

            return failures;
        }

        private static ValidationError[] MapValidationFailuresToValidationDomainErrors(LibraryFluentValidationResult libFluentResult)
        {
            ValidationError[] errors = new ValidationError[libFluentResult.Errors.Count];

            for (int i = 0; i < libFluentResult.Errors.Count; i++)
            {
                var error = libFluentResult.Errors[i];

                ErrorCode errorCode;
                try
                {
                    errorCode = ErrorCode.ConstructFromStringRepresentation(error.ErrorCode);
                }
                catch
                {
                    errorCode = ConstructAlternativeCode(error.ErrorCode);
                }

                errors[i] = new(errorCode, error.ErrorMessage);
            }

            return errors;
        }

        private static ErrorCode ConstructAlternativeCode(string errorCode)
        {
            try
            {
                return ErrorCode.ConstructFromCombinedTypeDomain(errorCode, "UnSpecified");
            }
            catch
            {
                return ErrorCode.ConstructFromCombinedTypeDomain(ValidationErrorCodeFactory.UnknownErrorCode, "UnSpecified");
            }
        }
    }
}
