using Domain.CustomFluentValidation;
using Domain.Entities;
using FluentValidation;

namespace Application.CQRS.BookCQRS.Commands.CreateBook
{
    public class CreateBookCommandValidator : LibraryValidator<CreateBookCommand>
    {
        public CreateBookCommandValidator()
            : base(LibraryValidatorType.Request)
        {
            // These rules are copied from Book Validator because at the moment its just a 1:1 map.
            RuleFor(book => book.Author).NotEmpty();

            RuleFor(book => book.Title).NotEmpty().MaximumLength(Book.MAX_LENGTH_TITLE);

            RuleFor(book => book.Description).NotEmpty();

            RuleFor(book => book.Pages).GreaterThan(0);

            RuleFor(book => book.DatePublished).NotEmpty().LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));

            RuleFor(book => book.Publisher).NotEmpty().MaximumLength(Book.MAX_LENGTH_PUBLISHER);

            RuleFor(book => book.DateRecieved).NotEmpty().LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));
        }
    }
}
