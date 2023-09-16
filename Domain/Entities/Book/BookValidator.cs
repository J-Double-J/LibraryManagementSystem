using Domain.CustomFluentValidation;
using FluentValidation;

namespace Domain.Entities
{
    public class BookValidator : LibraryValidator<Book>
    {
        public BookValidator() : base(LibraryValidatorType.Entity)
        {
            RuleFor(book => book.Author).NotEmpty();

            RuleFor(book => book.Title).NotEmpty().MaximumLength(Book.MAX_LENGTH_TITLE);

            RuleFor(book => book.Description).NotEmpty();

            RuleFor(book => book.Pages).GreaterThan(0);

            RuleFor(book => book.DatePublished).NotEmpty();

            RuleFor(book => book.Publisher).MaximumLength(Book.MAX_LENGTH_PUBLISHER);

            RuleFor(book => book.DateRecieved).NotEmpty().LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));
        }
    }
}
