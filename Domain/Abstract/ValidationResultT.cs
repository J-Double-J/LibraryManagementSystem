namespace Domain.Abstract
{
    public class ValidationResult<TValue> : Result<TValue>, IValidationResult
    {
        private ValidationResult(ValidationError[] errors)
            : base(default, false, IValidationResult.ValidationError)
        {
            ValidationErrors = errors;
        }

        public ValidationError[] ValidationErrors { get; }

        public static ValidationResult<TValue> WithErrors(ValidationError[] errors) { return new ValidationResult<TValue>(errors); }
    }
}
