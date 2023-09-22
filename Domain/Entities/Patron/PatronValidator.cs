using Domain.CustomFluentValidation;
using FluentValidation;

namespace Domain.Entities.Patron
{
    public class PatronValidator : LibraryValidator<Patron>
    {
        public PatronValidator()
            : base(LibraryValidatorType.Entity)
        {
            RuleFor(patron => patron.FirstName).NotNull().NotEmpty().MaximumLength(Patron.PATRON_NAME_MAXLENGTH);
            RuleFor(patron => patron.LastName).NotNull().NotEmpty().MaximumLength(Patron.PATRON_NAME_MAXLENGTH);
            RuleFor(patron => patron.Age).GreaterThanOrEqualTo(Patron.MINIMUM_AGE).LessThan(256);
        }
    }
}
