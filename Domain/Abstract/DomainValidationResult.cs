using Newtonsoft.Json;
using System.Text;

namespace Domain.Abstract
{
    public class DomainValidationResult : Result, IValidationResult
    {
        private string? _highLevelErrorCode;

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

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? HighLevelErrorCode
        {
            get
            {
                if (!string.IsNullOrEmpty(_highLevelErrorCode))
                {
                    return _highLevelErrorCode;
                }

                if (ValidationErrors.Length > 0)
                {
                    return ValidationErrors[0];
                }

                return null;
            }

            set
            {
                if (value is null)
                {
                    throw new InvalidOperationException("Cannot set HighLevelErrorCode to null");
                }

                _highLevelErrorCode = value;
            }
        }

        public ValidationError[] ValidationErrors { get; }

        public static new DomainValidationResult Success() { return SuccessfulValidation(); }

        public static DomainValidationResult SuccessfulValidation() { return new DomainValidationResult(); }

        public static DomainValidationResult WithErrors(ValidationError[] errors) { return new DomainValidationResult(errors); }
    }
}
