using Domain.Primitives;
using System.Text;

namespace Domain.CustomFluentValidation
{
    /// <summary>
    /// This enum clarifies at what level of the application the validator is concerned with.
    /// This also means that all validation rules internally should be as relevant to the level as possible.
    /// </summary>
    public enum LibraryValidatorType
    {
        None = 0,

        Entity = 10,

        Database = 20,

        ExternalCommunication = 30,

        IO = 40,

        Configuration = 50,

        // Mixed means that a validator could have various levels and the error code will be
        // set at the validation result level.
        Mixed = 98,

        Unknown = 99
    }

    public static class LibraryValidationErrorCodeHelper
    {
        private const string ValidationCodeRoot = "ValidationFailure";
        public const string EntityErrorCode = $"{ValidationCodeRoot}.Entity";
        public const string DatabaseErrorCode = $"{ValidationCodeRoot}.Database";
        public const string ExternalCommunicationErrorCode = $"{ValidationCodeRoot}.ExternalCommunication";
        public const string IOErrorCode = $"{ValidationCodeRoot}.IO";
        public const string ConfigurationErrorCode = $"{ValidationCodeRoot}.Configuration";
        public const string UnknownErrorCode = $"{ValidationCodeRoot}.Unknown";

        private static KeyValuePair<LibraryValidatorType, string>[] libraryValidatorLevelToString = new[]
        {
            new KeyValuePair<LibraryValidatorType, string>(LibraryValidatorType.Entity, EntityErrorCode),
            new KeyValuePair<LibraryValidatorType, string>(LibraryValidatorType.Database, DatabaseErrorCode),
            new KeyValuePair<LibraryValidatorType, string>(LibraryValidatorType.ExternalCommunication, ExternalCommunicationErrorCode),
            new KeyValuePair<LibraryValidatorType, string>(LibraryValidatorType.IO, IOErrorCode),
            new KeyValuePair<LibraryValidatorType, string>(LibraryValidatorType.Unknown, UnknownErrorCode),
        };

        public static BidirectionalDictionary<LibraryValidatorType, string> ValidatorAndStringBidirectionalDict = new(libraryValidatorLevelToString);
        
        /// <summary>
        /// Constructs a valid Library Validation Error Code
        /// </summary>
        /// <param name="validationType">Type of validation that failed</param>
        /// <param name="specifiedErrorCode">End of error code that specifies what exactly went wrong.</param>
        /// <returns>A constructed error code following Domain rules.</returns>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="specifiedErrorCode"/> is null.</exception>
        public static string ConstructErrorCode(LibraryValidatorType validationType, string specifiedErrorCode)
        {
            if (string.IsNullOrEmpty(specifiedErrorCode))
            {
                throw new ArgumentNullException(nameof(specifiedErrorCode), "Specified Error Code cannot be null or empty." +
                    "If you must construct an error code try calling `PrefixErrorCodeIfNoPrefixAttached()` to track LibraryValidatorType");
            }

            return PrefixErrorCodeIfNoPrefixAttached(validationType, specifiedErrorCode);
        }

        /// <summary>
        /// Attaches the Validation error prefix if there is no prefix attached. If <paramref name="errorCode"/> is null, its set to "NonSpecified"
        /// </summary>
        /// <param name="validationType">LType of validation that failed.</param>
        /// <param name="errorCode">Error code to prefix</param>
        /// <returns>A constructed error code following Domain rules.</returns>
        public static string PrefixErrorCodeIfNoPrefixAttached(LibraryValidatorType validationType, string errorCode)
        {
            if (!string.IsNullOrEmpty(errorCode) && errorCode.StartsWith(ValidationCodeRoot))
            {
                return errorCode;
            }

            if (string.IsNullOrEmpty(errorCode))
            {
                errorCode = "NonSpecific";    
            }

            return validationType switch
            {
                LibraryValidatorType.None => errorCode,
                LibraryValidatorType.Entity => $"{EntityErrorCode}.{errorCode}",
                LibraryValidatorType.Database => $"{DatabaseErrorCode}.{errorCode}",
                LibraryValidatorType.ExternalCommunication => $"{ExternalCommunicationErrorCode}.{errorCode}",
                LibraryValidatorType.IO => $"{IOErrorCode}.{errorCode}",
                LibraryValidatorType.Configuration => $"{ConfigurationErrorCode}.{errorCode}",
                _ => $"{UnknownErrorCode}.{errorCode}",
            };
        }

        public static LibraryValidatorType ValidationLevelFromCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new InvalidOperationException("Code must be non-empty to obtain ValidationLevel");
            }

            int firstIndexOfDot = code.IndexOf('.');
            int lastIndexOfDot = code.LastIndexOf('.');

            if (firstIndexOfDot == -1 || lastIndexOfDot == firstIndexOfDot)
            {
                throw new ArgumentException("Code must be given in the standard of errorType.MoreSpecific.MostSpecific");
            }

            string prefix = code[..lastIndexOfDot];

            if (ValidatorAndStringBidirectionalDict.TryGetBySecondKey(prefix, out var validationLevel))
            {
                return validationLevel;
            }

            return LibraryValidatorType.Unknown;
        }
    }
}
