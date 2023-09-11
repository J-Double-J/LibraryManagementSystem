using Domain.Entities;
using MediatR;

namespace Application.CQRS.BookCQRS.Commands
{
    public class CreateBookCommand :IRequest<Book>
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

        public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, Book>
        {
            private readonly IBookRepository _repository;
            public CreateBookCommandHandler(IBookRepository bookRepository)
            {
                _repository = bookRepository;
            }

            public async Task<Book> Handle(CreateBookCommand command, CancellationToken cancellationToken)
            {
                Book book = Book.Create(Guid.NewGuid(),
                    command.Author,
                    command.Title,
                    command.Description,
                    command.Pages,
                    command.DatePublished,
                    command.Publisher,
                    command.DateRecieved);

                await _repository.AddBook(book);

                return book;
            }
        }
    }
}
