using Domain.CustomFluentValidation;
using Newtonsoft.Json;

namespace Domain.Abstract
{
    public class DomainValidationResult<TValue> : Result<TValue>, IValidationResult
    {
        private string? _highLevelErrorCode;

        private DomainValidationResult(ValidationError[] errors)
            : base(default, false, IValidationResult.ValidationError)
        {
            ValidationErrors = errors;
        }

        private DomainValidationResult(TValue value)
            : base(value, true, Error.None)
        {
            ValidationErrors = Array.Empty<ValidationError>();
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? HighLevelErrorCode {
            get
            {
                if (!string.IsNullOrEmpty(_highLevelErrorCode))
                {
                    return _highLevelErrorCode;
                }

                if(ValidationErrors.Length > 0)
                {
                    return null;
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

        public static DomainValidationResult<TValue> WithErrors(ValidationError[] errors) { return new DomainValidationResult<TValue>(errors); }
        public static DomainValidationResult<TValue> SuccessfulValidation(TValue value) { return new DomainValidationResult<TValue>(value); }

        public bool ShouldSerializeError()
        {
            return false;
        }
    }
}
