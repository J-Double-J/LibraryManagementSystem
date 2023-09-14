namespace Infrastructure.Configuration
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string configurationParameterName, string message)
            : base(message)
        {
            MissingConfigurationParameter = configurationParameterName;
        }

        public string MissingConfigurationParameter { get; private init; }
    }
}
