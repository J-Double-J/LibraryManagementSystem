using Domain.Abstract;
using Domain.Errors;
using Domain.Exceptions;
using Domain.Primitives;
using Fluent = FluentValidation.Results;

namespace Domain.Entities
{
    public sealed class Book : Entity
    {
        public const int MAX_LENGTH_TITLE = 128;
        public const int MAX_LENGTH_PUBLISHER = 128;

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

        public static Result<Book> Create(
            string author,
            string title,
            string description,
            int pages,
            DateOnly datePublished,
            string publisher,
            DateOnly dateRecieved)
        {
            Book book = new(Guid.NewGuid(), author, title, description, pages, datePublished, publisher, dateRecieved);

            BookValidator bookValidator = new();

            Fluent.ValidationResult validationResult = bookValidator.Validate(book);

            if (!validationResult.IsValid)
            {
                string errorMessages = string.Join(Environment.NewLine, validationResult.Errors.Select(error => error.ErrorMessage));
                return Result.Failure<Book>(null, new("Book.CreateValidationError", errorMessages));
            }

            return book;
        }
    }
}
