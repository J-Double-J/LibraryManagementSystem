using Domain.CustomFluentValidation;
using Newtonsoft.Json;

namespace Domain.Abstract
{
    /// <summary>
    /// Class for Validation Errors.
    /// </summary>
    public class ValidationError : Error
    {

        #region Constructors

        /// <summary>
        /// Validation Error for constructor.
        /// </summary>
        /// <param name="messageHeader">String to display in message before error. Note that a new line is inserted at the end.</param>
        /// <param name="errorMessage">Error message for validation.</param>
        public ValidationError(string errorCode, string messageHeader, string errorMessage)
            : base(errorCode, string.Concat(messageHeader, Environment.NewLine, errorMessage))
        {
            ValidationMessages = new string[] { errorMessage };

            DetermineIfValidationLevelIsInternal(errorCode);
        }

        public ValidationError(string errorCode, string messageHeader, string errorMessage, bool isInternal)
            : base(errorCode, string.Concat(messageHeader, Environment.NewLine, errorMessage, isInternal))
        {
            ValidationMessages = new string[] { errorMessage };
        }

        /// <summary>
        /// Validation Error constructor that has a message before listing out all validation error messages.
        /// </summary>
        /// <param name="messageHeader">String to display in message before errors. Note that a new line is inserted at the end.</param>
        /// <param name="validationErrorMessages">Messages to display for each validation error seperated by new lines.</param>
        public ValidationError(string errorCode, string messageHeader, string[] validationErrorMessages) 
            : base(errorCode,
                   string.Concat(messageHeader, Environment.NewLine, string.Join(Environment.NewLine, validationErrorMessages)))
        {
            ValidationMessages = validationErrorMessages;
        }

        public ValidationError(string errorCode, string messageHeader, string[] validationErrorMessages, bool isInternal)
            : base(errorCode,
                   string.Concat(messageHeader, Environment.NewLine, string.Join(Environment.NewLine, validationErrorMessages)),
                   isInternal)
        {
            ValidationMessages = validationErrorMessages;

            DetermineIfValidationLevelIsInternal(errorCode);
        }

        /// <summary>
        /// Validation Error constructor that lists out all validation error messages.
        /// </summary>
        /// <param name="validationErrorMessages">Messages to display for each validation error seperated by new lines.</param>
        public ValidationError(string errorCode, string[] validationErrorMessages) 
            : base(errorCode, string.Join(Environment.NewLine, validationErrorMessages))
        {
            ValidationMessages = validationErrorMessages;

            DetermineIfValidationLevelIsInternal(errorCode);
        }

        public ValidationError(LibraryValidatorType validationLevel, string[] validationErrorMessages, bool isInternal)
            : base(validationLevel.ToString(), string.Join(Environment.NewLine, validationErrorMessages), isInternal)
        {
            ValidationMessages = validationErrorMessages;
        }

        #endregion Constructors

        /// <summary>
        /// Gets all validation error messages.
        /// </summary>
        public string[] ValidationMessages { get; init; }

        [JsonIgnore]
        public LibraryValidatorType ValidationErrorLevel { get; init; }

        /// <summary>
        /// Determines if the error code type is an internal error. This can be used to mask errors when returned in the API.
        /// </summary>
        /// <param name="validationLevel">What level the error occurred at.</param>
        /// <exception cref="InvalidOperationException">Throws if a <see cref="LibraryValidatorType.Mixed"/> is passed in. Errors
        /// must </exception>
        private void DetermineIfValidationLevelIsInternal(string errorCode)
        {
            LibraryValidatorType validatorLevel = LibraryValidationErrorCodeHelper.ValidationLevelFromCode(errorCode);

            // Likely subject to change at some point as errors become more specific. These enums might be extracted to classes to have behavior.
            switch (validatorLevel)
            {
                case LibraryValidatorType.None:        IsInternalError = false; break;
                case LibraryValidatorType.Entity:      IsInternalError = false; break;
                case LibraryValidatorType.Database:    IsInternalError = true; break;
                case LibraryValidatorType.ExternalCommunication:   IsInternalError = false; break;
                case LibraryValidatorType.IO:                      IsInternalError = true; break;
                case LibraryValidatorType.Configuration:           IsInternalError = true; break;
                case LibraryValidatorType.Mixed:       throw new InvalidOperationException("Validation Errors must not be mixed and must be assigned.");
                case LibraryValidatorType.Unknown:     IsInternalError = true; break; // We assume we did not account for an internal problem, or code was improperly formmatted.
            }
        }
    }
}
