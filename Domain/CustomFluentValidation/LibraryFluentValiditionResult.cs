using Fluent = FluentValidation.Results;

namespace Domain.CustomFluentValidation
{
    public class LibraryFluentValidationResult : Fluent.ValidationResult
    {
        public LibraryFluentValidationResult() { Errors = new(); }

        public LibraryValidatorType ValidationLevel { get; set; }
    }
}
