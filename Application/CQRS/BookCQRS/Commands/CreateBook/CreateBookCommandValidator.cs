using FluentValidation;

namespace Application.CQRS.BookCQRS.Commands.CreateBook
{
    public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
    {
        public CreateBookCommandValidator()
        {
            RuleFor(book => book.Author).NotEmpty();

            RuleFor(book => book.Title).NotEmpty().MaximumLength(CreateBookCommand.MAX_LENGTH_TITLE);

            RuleFor(book => book.Description).NotEmpty();

            RuleFor(book => book.Pages).GreaterThan(0);

            RuleFor(book => book.DatePublished).NotEmpty();

            RuleFor(book => book.Publisher).MaximumLength(CreateBookCommand.MAX_LENGTH_PUBLISHER);

            RuleFor(book => book.DateRecieved).NotEmpty().LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));

        }
    }
}
