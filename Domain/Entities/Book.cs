using Domain.Abstract;
using Domain.Errors;
using Domain.Exceptions;
using Domain.Primitives;

namespace Domain.Entities
{
    public sealed class Book : Entity
    {
        private Book(Guid id,
            string author,
            string title,
            string description,
            int pages,
            DateOnly datePublished,
            string publisher,
            DateOnly dateRecieved)
            : base(id)
        {
            Author = author;
            Title = title;
            Description = description;
            Pages = pages;
            DatePublished = datePublished;
            Publisher = publisher;
            DateRecieved = dateRecieved;
        }

        /// <summary>
        /// Gets the Author of the book.
        /// </summary>
        public string Author { get; init; }

        /// <summary>
        /// Gets the title of the book.
        /// </summary>
        public string Title { get; init; }

        /// <summary>
        /// Gets the description of the book.
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// Gets the number of pages in the book.
        /// </summary>
        public int Pages { get; init; }

        /// <summary>
        /// Gets the date the book was published.
        /// </summary>
        public DateOnly DatePublished { get; init; }

        /// <summary>
        /// Gets the publisher of the book.
        /// </summary>
        public string Publisher { get; init; }

        /// <summary>
        /// Gets the date the book was recieved into the system.`
        /// </summary>
        public DateOnly DateRecieved { get; init; }

        public static Book Create(Guid id,
            string author,
            string title,
            string description,
            int pages,
            DateOnly datePublished,
            string publisher,
            DateOnly dateRecieved)
        {
            Book book = new(id, author, title, description, pages, datePublished, publisher, dateRecieved);

            Result validationResult = ValidateBook(book);

            if(validationResult.IsFailure)
            {
                throw new ArgumentException(validationResult.Error?.Message, nameof(pages));
            }

            return book;
        }

        private static Result ValidateBook(Book book)
        {
            if (book.Pages < 0)
            {
                return Result.Failure(DomainErrors.Book.NegativeOrZeroPages);
            }

            return Result.Success();
        }
    }
}
