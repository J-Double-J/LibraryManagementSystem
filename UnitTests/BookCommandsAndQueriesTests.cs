using Application;
using Application.CQRS.BookCQRS.Commands;
using Application.CQRS.BookCQRS.Commands.CreateBook;
using Application.CQRS.BookCQRS.Commands.RemoveBook;
using Application.CQRS.BookCQRS.Commands.UpdateBook;
using Application.CQRS.BookCQRS.Queries;
using Domain.Abstract;
using Domain.Entities;
using FluentAssertions;
using FluentValidation.Results;
using NSubstitute;
using System.Collections;

namespace UnitTests
{
    public class BookCommandsAndQueriesTests
    {
        public class CreateBookComamndInvalidValues : IEnumerable<object[]>
        {
            public CreateBookComamndInvalidValues()
            {
            }

            // Valid values
            private static readonly DateTime today = DateTime.UtcNow;
            private static readonly DateOnly yesterday = DateOnly.FromDateTime(today.AddDays(-1));
            private static readonly DateOnly tomorrow = DateOnly.FromDateTime(today.AddDays(1));
            private const string author = "Author";
            private const string title = "Title";
            private const string description = "Description";
            private const int pages = 10;
            private const string publisher = "Publisher";

            // Invalid commands
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { new CreateBookCommand("", title, description, pages, yesterday, publisher, yesterday) };
                yield return new object[] { new CreateBookCommand(author, "", description, pages, yesterday, publisher, yesterday) };
                yield return new object[] { new CreateBookCommand(author, title, "", pages, yesterday, publisher, yesterday) };
                yield return new object[] { new CreateBookCommand(author, title, description, -1, yesterday, publisher, yesterday) };
                yield return new object[] { new CreateBookCommand(author, title, description, pages, tomorrow, publisher, yesterday) };
                yield return new object[] { new CreateBookCommand(author, title, description, pages, yesterday, "", yesterday) };
                yield return new object[] { new CreateBookCommand(author, title, description, pages, yesterday, publisher, tomorrow) };
                yield return new object[] { new CreateBookCommand(author, new('a', Book.MAX_LENGTH_TITLE + 1), description, pages, yesterday, publisher, yesterday) };
                yield return new object[] { new CreateBookCommand(author, title, description, pages, yesterday, new('a', Book.MAX_LENGTH_PUBLISHER + 1), yesterday) };

            }


            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Fact]
        public async void ValidBookGetAllBooksQuery_SuccessfullyGetsAllBooks()
        {
            DateOnly yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
            Book validBook = Book.Create("author", "title", "description", 10, yesterday, "publisher", yesterday).Result.Value;
            Book[] books = { validBook, validBook, validBook };

            IBookRepository bookRepository = Substitute.For<IBookRepository>();
            GetAllBooksQuery query = new();
            GetAllBooksQuery.GetAllBooksQueryHandler handler = new(bookRepository);
            bookRepository.GetAllBooks().Returns(books);

            Result<IEnumerable<Book>> result = await handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(books.Length);
        }

        [Fact]
        public async void ErrorInRepository_PassesThroughGetAllBooksQuery()
        {
            IBookRepository bookRepository = Substitute.For<IBookRepository>();
            GetAllBooksQuery query = new();
            GetAllBooksQuery.GetAllBooksQueryHandler handler = new(bookRepository);

            bookRepository.GetAllBooks().Returns(Result.Failure(Enumerable.Empty<Book>(),
                TestReferenceHelper.TEST_ERROR));

            Result<IEnumerable<Book>> result = await handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(TestReferenceHelper.TEST_ERROR);
        }

        [Fact]
        public async void GetBookByIDCommand_CanGetValidBook()
        {
            DateOnly yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
            Book validBook = Book.Create("author", "title", "description", 10, yesterday, "publisher", yesterday).Result.Value;
            IBookRepository bookRepository = Substitute.For<IBookRepository>();

            GetBookByIDQuery query = new(validBook.Id);
            GetBookByIDQuery.GetBookByIDQueryHandler handler = new(bookRepository);

            bookRepository.GetBookByID(validBook.Id).Returns(validBook);

            Result<Book> result = await handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(validBook);
        }

        [Fact]
        public async void GetBookByIDCommand_WillBubbleUpFailureResult()
        {
            IBookRepository bookRepository = Substitute.For<IBookRepository>();
            Guid searchGuid = Guid.NewGuid();
            GetBookByIDQuery query = new(searchGuid);
            GetBookByIDQuery.GetBookByIDQueryHandler handler = new(bookRepository);

            bookRepository.GetBookByID(searchGuid).Returns(Result.Failure<Book>(null, TestReferenceHelper.TEST_ERROR));

            Result<Book> result = await handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeEquivalentTo(TestReferenceHelper.TEST_ERROR);
        }

        [Fact]
        public async void CreateBookCommand_CanSuccessfullyCreateBook()
        {
            string author = "Author";
            DateOnly yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
            CreateBookCommand command = new(author, "Title", "Description", 10, yesterday, "publisher", yesterday);

            IBookRepository bookRepository = Substitute.For<IBookRepository>();
            CreateBookCommand.CreateBookCommandHandler handler = new(bookRepository);

            bookRepository.AddBook(Arg.Any<Book>()).Returns(Result.Success());

            Result<Book> result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType(typeof(Book));
            result.Value.Author.Should().Be(author);
        }

        [Theory]
        [ClassData(typeof(CreateBookComamndInvalidValues))]
        public async void CreateBookCommand_CannotCreateInvalidCommand(CreateBookCommand command)
        {
            CreateBookCommandValidator validator = new();

            DomainValidationResult<CreateBookCommand> result = await validator.ValidateAsyncGetDomainResult(command);

            result.IsFailure.Should().BeTrue();
            result.ValidationErrors.Should().HaveCount(1);
        }

        [Fact]
        public async void RemoveBookCommand_CanBeSuccessful()
        {
            IBookRepository bookRepository = Substitute.For<IBookRepository>();
            RemoveBookCommand command = new(Guid.NewGuid());
            RemoveBookCommand.RemoveBookCommandHandler handler = new(bookRepository);

            bookRepository.DeleteBook(Arg.Any<Guid>()).Returns(Result.Success());

            Result result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async void RemoveBookCommand_CanBeAFailure()
        {
            IBookRepository bookRepository = Substitute.For<IBookRepository>();
            RemoveBookCommand command = new(Guid.NewGuid());
            RemoveBookCommand.RemoveBookCommandHandler handler = new(bookRepository);

            bookRepository.DeleteBook(Arg.Any<Guid>()).Returns(Result.Failure(TestReferenceHelper.TEST_ERROR));

            Result result = await handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeEquivalentTo(TestReferenceHelper.TEST_ERROR);
        }
    }
}
