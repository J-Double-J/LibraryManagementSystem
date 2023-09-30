using Application.Interfaces.Configuration;
using Infrastructure.Configuration.Objects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Configuration
{
    public class LibraryManagementConfiguration : ILibraryManagementConfiguration
    {
        private const string KEY_MAXIMUM_NUM_COPIES = "MaximumNumberOfCopiesOfBook";
        private const string RENEWAL_POLICY_SECTION = "RenewalPolicy";

        IConfiguration _configuration;
        ILogger<LibraryManagementConfiguration> _logger;

        public LibraryManagementConfiguration(IConfiguration configuration, ILogger<LibraryManagementConfiguration> logger)
        {
            _configuration = configuration;
            _logger = logger;

            SetMaximumNumOfBooksConfiguration();
            SetRenewalPolicy();
        }

        public int MaximumNumberOfCopiesOfBook { get; private set; }

        public IRenewalPolicy RenewalPolicy { get; private set; }

        private void SetMaximumNumOfBooksConfiguration()
        {
            string? value = _configuration[KEY_MAXIMUM_NUM_COPIES];

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ConfigurationException(nameof(MaximumNumberOfCopiesOfBook),
                    $"Configuration value `{KEY_MAXIMUM_NUM_COPIES}` is empty or not found. Value must be set.");
            }

            if (!int.TryParse(value, out int result))
            {
                throw new ConfigurationException(nameof(MaximumNumberOfCopiesOfBook),
                    $"Configuration value `{KEY_MAXIMUM_NUM_COPIES}` is not an integer. Change value to an integer.");
            }

            MaximumNumberOfCopiesOfBook = result;
        }


        private void SetRenewalPolicy()
        {
            try
            {
                RenewalPolicy policy = new();
                _configuration.GetSection(RENEWAL_POLICY_SECTION).Bind(policy);

                var result = policy.Validate();

                if (result.Errors.Count > 0)
                {
                    string propertyName = result.Errors[0].PropertyName;
                    throw new ConfigurationException(propertyName, "Renewal Policy failed validation and has incorrectly configured values.");
                }

                RenewalPolicy = policy;
            }
            catch
            {
                _logger.LogWarning("Configuration failed to load in RenewalPolicy. Creating default policy.");
                RenewalPolicy = new RenewalPolicy() { MaximumRenewals = 1, RenewalPeriodInDays = 7 };
            }
        }
    }
}
