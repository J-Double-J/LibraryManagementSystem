namespace Domain.Abstract
{
    public class ValidationError : Error
    {
        /// <summary>
        /// Validation Error constructor that has a message before listing out all validation error messages.
        /// </summary>
        /// <param name="messageHeader">String to display in message before errors. Note that a new line is inserted at the end.</param>
        /// <param name="validationErrorMessages">Messages to display for each validation error seperated by new lines.</param>
        public ValidationError(string messageHeader, string[] validationErrorMessages) 
            : base("ValidationError",
                   string.Concat(messageHeader, Environment.NewLine, string.Join(Environment.NewLine, validationErrorMessages)))
        {
            ValidationMessages = validationErrorMessages;
        }

        /// <summary>
        /// Validation Error constructor that lists out all validation error messages.
        /// </summary>
        /// <param name="validationErrorMessages">Messages to display for each validation error seperated by new lines.</param>
        public ValidationError(string[] validationErrorMessages) : base("ValidationError", string.Join(Environment.NewLine, validationErrorMessages))
        {
            ValidationMessages = validationErrorMessages;
        }

        /// <summary>
        /// Gets all validation error messages.
        /// </summary>
        public string[] ValidationMessages { get; init; }
    }
}
