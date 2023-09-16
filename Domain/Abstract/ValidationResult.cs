using System.Text;

namespace Domain.Abstract
{
    public class DomainValidationResult : Result, IValidationResult
    {
        private DomainValidationResult(ValidationError[] errors) : base(false, IValidationResult.ValidationError)
        {
            ValidationErrors = errors;
        }

        /// <summary>
        /// Successful Validation Para
        /// </summary>
        /// <param name="success"></param>
        /// <param name="errorType"></param>
        private DomainValidationResult()
            : base(true, Error.None)
        {
            ValidationErrors = Array.Empty<ValidationError>(); 
        }

        public ValidationError[] ValidationErrors { get; }

        public string FlattenedErrorMessage {
            get
            {
                if (ValidationErrors.Length == 0)
                {
                    throw new InvalidOperationException("There are no error messages to flatten");
                }

                StringBuilder sb = new StringBuilder();

                foreach (var error in ValidationErrors)
                {
                    foreach (string message in error.ValidationMessages)
                    {
                        sb.AppendLine(message);
                    }
                }

                return sb.ToString();
            }
        }

        public static new DomainValidationResult Success() { return SuccessfulValidation(); }

        public static DomainValidationResult SuccessfulValidation() { return new DomainValidationResult(); }

        public static DomainValidationResult WithErrors(ValidationError[] errors) { return new DomainValidationResult(errors); }
    }
}
