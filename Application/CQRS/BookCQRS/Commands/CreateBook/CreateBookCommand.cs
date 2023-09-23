using Domain.Abstract;
using Domain.CustomFluentValidation;
using Domain.Entities;

namespace Application.CQRS.BookCQRS.Commands
{
    public class CreateBookCommand : ICommand<Book>
    {
        public string Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Pages { get; set; }
        public DateOnly DatePublished { get; set; }
        public string Publisher { get; set; }
        public DateOnly DateRecieved { get; set; }

        public CreateBookCommand(
            string author,
            string title,
            string description,
            int pages,
            DateOnly datePublished,
            string publisher,
            DateOnly dateRecieved)
        {
            Author = author;
            Title = title;
            Description = description;
            Pages = pages;
            DatePublished = datePublished;
            Publisher = publisher;
            DateRecieved = dateRecieved;
        }

        public class CreateBookCommandHandler : ICommandHandler<CreateBookCommand, Book>
        {
            private readonly IBookRepository _repository;
            public CreateBookCommandHandler(IBookRepository bookRepository)
            {
                _repository = bookRepository;
            }

            public async Task<Result<Book>> Handle(CreateBookCommand command, CancellationToken cancellationToken)
            {
                Result<Book> bookResult = await Book.Create(
                    command.Author,
                    command.Title,
                    command.Description,
                    command.Pages,
                    command.DatePublished,
                    command.Publisher,
                    command.DateRecieved);

                if(bookResult.IsSuccess)
                {
                    Result dbResult = await _repository.AddBook(bookResult.Value);

                    if (dbResult.IsSuccess)
                    {
                        return bookResult;
                    }

                    return Result.Failure<Book>(null, dbResult.Error!);
                }

                if (bookResult is DomainValidationResult<Book> domainResult)
                {
                    domainResult.HighLevelErrorCode = ValidationErrorCodeFactory.ConstructErrorCode(LibraryValidatorType.Entity, "BookCreation");
                    return domainResult;
                }
                
                // Shouldn't be reached.
                return bookResult;
            }
        }
    }
}
