using FluentValidation.Results;
using Fluent = FluentValidation.Results;

namespace Domain.CustomFluentValidation
{
    public class LibraryFluentValidationResult : Fluent.ValidationResult
    {
        public LibraryFluentValidationResult() { Errors = new(); }

        public LibraryFluentValidationResult(ValidationResult result, LibraryValidatorType libraryValidatorType)
        {
            Errors = result.Errors;
            LibraryValidatorType = libraryValidatorType;
        }

        public LibraryValidatorType LibraryValidatorType { get; set; }
    }
}
