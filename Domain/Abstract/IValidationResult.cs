namespace Domain.Abstract
{
    public interface IValidationResult
    {
        public static readonly Error ValidationError = new("ValidationError", "A validation error occurred.");

        ValidationError[] ValidationErrors { get; }
    }
}
