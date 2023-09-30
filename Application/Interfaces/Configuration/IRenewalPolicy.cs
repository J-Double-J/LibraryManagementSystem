namespace Application.Interfaces.Configuration
{
    public interface IRenewalPolicy
    {
        public int MaximumRenewals { get; init; }
        public int RenewalPeriodInDays { get; init; }
    }
}