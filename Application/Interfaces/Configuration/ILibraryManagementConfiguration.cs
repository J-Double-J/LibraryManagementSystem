namespace Application.Interfaces.Configuration
{
    public interface ILibraryManagementConfiguration
    {
        public int MaximumNumberOfCopiesOfBook { get; }
        public IRenewalPolicy RenewalPolicy { get; }
    }
}
