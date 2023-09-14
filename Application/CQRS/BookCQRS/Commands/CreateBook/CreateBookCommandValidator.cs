using Application.Interfaces.Configuration;
using Domain.Abstract;
using Domain.Entities;
using FluentValidation;

namespace Application.CQRS.BookCQRS.Commands.CreateBook
{
    public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILibraryManagementConfiguration _configuration;
        
        public CreateBookCommandValidator(IBookRepository bookRepository, ILibraryManagementConfiguration configuration)
        {
            _bookRepository = bookRepository;
            _configuration = configuration;

            RuleFor(command => command).CustomAsync(BeABookThatWillBeTakenIntoSystem);
        }

        private async Task BeABookThatWillBeTakenIntoSystem(CreateBookCommand command,
                                                            ValidationContext<CreateBookCommand> context,
                                                            CancellationToken cancellationToken)
        {
            Result<IEnumerable<Book>> result = await _bookRepository.GetBooksByTitle(command.Title);

            if (result.IsFailure)
            {
                context.AddFailure(DatabaseValidationErrorName.DATABASE_ERROR, $"Failed to validate book: {result.Error!.Message}");
                return;
            }

            IEnumerable<Book> booksWithSameAuthorAndTitle = result.Value.Where(book => book.Author == command.Author);

            if (booksWithSameAuthorAndTitle.Count() >= _configuration.MaximumNumberOfCopiesOfBook)
            {
                context.AddFailure(nameof(command.Title),
                    $"Library already has {_configuration.MaximumNumberOfCopiesOfBook} copies of this book and will not be able to accept another.");
            }
        }
    }
}
