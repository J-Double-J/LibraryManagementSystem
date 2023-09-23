using Domain.Abstract;
using Domain.Entities;

namespace Application.CQRS.BookCQRS.Commands.UpdateBook
{
    public class UpdateBookCommand : ICommand<Book>
    {
        public Guid BookToUpdateID { get; init; }
        public string? Author { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? Pages { get; set; }
        public DateOnly? DatePublished { get; set; }
        public string? Publisher { get; set; }

        public UpdateBookCommand(
            Guid guid,
            string? author,
            string? title,
            string? description,
            int? pages,
            DateOnly? datePublished,
            string? publisher
            )
        {
            Author = author;
            Title = title;
            Description = description;
            Pages = pages;
            DatePublished = datePublished;
            Publisher = publisher;
        }

        public class UpdateBookCommandHandler : ICommandHandler<UpdateBookCommand, Book>
        {
            private readonly IBookRepository _bookRepository;

            public UpdateBookCommandHandler(IBookRepository bookRepository)
            {
                _bookRepository = bookRepository;
            }

            public async Task<Result<Book>> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
            {
                Result<Book> bookResult = await _bookRepository.GetBookByID(request.BookToUpdateID);

                if(bookResult.IsFailure)
                {
                    return bookResult;
                }

                Book curBook = bookResult.Value;
                Result<Book> newBookResult = await Book.Create(request.Author ?? curBook.Author,
                                                         request.Title ?? curBook.Title,
                                                         request.Description ?? curBook.Description,
                                                         request.Pages ?? curBook.Pages,
                                                         request.DatePublished ?? curBook.DatePublished,
                                                         request.Publisher ?? curBook.Publisher,
                                                         curBook.DateRecieved);

                if(newBookResult.IsFailure)
                {
                    return newBookResult;
                }

                Result<Book> updateResult = await _bookRepository.UpdateBook(request.BookToUpdateID, newBookResult.Value);

                return updateResult;
            }
        }
    }
}
