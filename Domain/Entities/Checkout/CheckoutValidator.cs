using Domain.CustomFluentValidation;
using FluentValidation;

namespace Domain.Entities
{
    public class CheckoutValidator : LibraryValidator<Checkout>
    {
        public CheckoutValidator() : base(LibraryValidatorType.Entity)
        {
            RuleFor(c => c.CheckoutDate).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));

            RuleFor(c => c.DueDate).GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow));

            RuleFor(c => c.RenewalsLeft).GreaterThanOrEqualTo(0);

            RuleFor(c => c.Patron).NotNull();

            RuleFor(c => c.Book).NotNull();
        }
    }
}
