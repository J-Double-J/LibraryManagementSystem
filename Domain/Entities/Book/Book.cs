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
        public string Author { get; set; }

        /// <summary>
        /// Gets the title of the book.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets the description of the book.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the number of pages in the book.
        /// </summary>
        public int Pages { get; set; }

        /// <summary>
        /// Gets the date the book was published.
        /// </summary>
        public DateOnly DatePublished { get; set; }

        /// <summary>
        /// Gets the publisher of the book.
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// Gets the date the book was recieved into the system.`
        /// </summary>
        public DateOnly DateRecieved { get; set; }

        public static async Task<Result<Book>>  Create(
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

            DomainValidationResult<Book> validationResult = await bookValidator.ValidateAsyncGetDomainResult(book);

            return validationResult;
        }

        /// <summary>
        /// Copies properties from one book to another.
        /// </summary>
        /// <param name="book">Book to copy properties to</param>
        public void CopyTo(Book book)
        {
            book.Author = Author;
            book.Title = Title;
            book.Description = Description;
            book.Pages = Pages;
            book.DatePublished = DatePublished;
            book.Publisher = Publisher;
            book.DateRecieved = DateRecieved;
        }
    }
}
