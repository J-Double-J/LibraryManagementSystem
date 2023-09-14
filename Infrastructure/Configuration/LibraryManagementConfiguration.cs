using Application.Interfaces.Configuration;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Configuration
{
    public class LibraryManagementConfiguration : ILibraryManagementConfiguration
    {
        private const string KEY_MAXIMUM_NUM_COPIES = "MaximumNumberOfCopiesOfBook";

        public LibraryManagementConfiguration(IConfiguration configuration)
        {
            GetMaximumNumOfBooksConfiguration(configuration);
        }

        private void GetMaximumNumOfBooksConfiguration(IConfiguration configuration)
        {
            string? value = configuration[KEY_MAXIMUM_NUM_COPIES];

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

        public int MaximumNumberOfCopiesOfBook { get; private set; }
    }
}
