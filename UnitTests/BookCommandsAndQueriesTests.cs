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
using Infrastructure;
using NSubstitute;
using System.Collections;

namespace UnitTests
{
    public class BookCommandsAndQueriesTests
    {
        public class CreateBookCommandInvalidValues : IEnumerable<object[]>
        {
            public CreateBookCommandInvalidValues()
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

        public class UpdateBookCommandInvalidValues : IEnumerable<object[]>
        {
            public UpdateBookCommandInvalidValues()
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
            private static readonly Guid guid = Guid.NewGuid();

            // Invalid commands. This scatters in some nulls because they should not matter for this command
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { new UpdateBookCommand(guid, "", title, description, pages, yesterday, null) };
                yield return new object[] { new UpdateBookCommand(guid, author, "", null, pages, yesterday, publisher) };
                yield return new object[] { new UpdateBookCommand(guid, null, title, "", pages, yesterday, publisher) };
                yield return new object[] { new UpdateBookCommand(guid, author, null, description, -1, yesterday, publisher) };
                yield return new object[] { new UpdateBookCommand(guid, author, title, null, pages, tomorrow, publisher) };
                yield return new object[] { new UpdateBookCommand(guid, author, title, description, null, yesterday, "") };
                yield return new object[] { new UpdateBookCommand(guid, author, new('a', Book.MAX_LENGTH_TITLE + 1), description, pages, null, publisher) };
                yield return new object[] { new UpdateBookCommand(guid, author, title, description, pages, yesterday, new('a', Book.MAX_LENGTH_PUBLISHER + 1)) };

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
        [ClassData(typeof(CreateBookCommandInvalidValues))]
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

        [Fact]
        public async void UpdateBookCommand_CanBeValidAndSuccessful()
        {
            // Set up
            IBookRepository bookRepository = Substitute.For<IBookRepository>();
            DateOnly yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
            UpdateBookCommand command = new(Guid.NewGuid(), "New Author", "title", "description", 10, yesterday, "publisher");
            UpdateBookCommand.UpdateBookCommandHandler handler = new(bookRepository);
            UpdateBookCommandValidator validator = new();
            Book book = (await Book.Create("author", "title", "description", 10, yesterday, "publisher", yesterday)).Value;

            // Validate command
            DomainValidationResult<UpdateBookCommand> validationResult = await validator.ValidateAsyncGetDomainResult(command, CancellationToken.None);

            // Implement substitute.
            bookRepository.GetBookByID(Arg.Any<Guid>()).Returns(Result.Success(book));
            bookRepository.UpdateBook(Arg.Any<Guid>(), Arg.Is<Book>(b => b.Author == "New Author")).Returns(Result.Success(book));
            bookRepository.UpdateBook(Arg.Any<Guid>(), Arg.Is<Book>(b => b.Author != "New Author")).Returns(Result.Failure<Book>(null, TestReferenceHelper.TEST_ERROR));

            // Handle and evaluate results
            Result result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            validationResult.IsSuccess.Should().BeTrue();
        }

        [Theory]
        [ClassData(typeof(UpdateBookCommandInvalidValues))]
        public async void UpdateBookCommand_CannotHaveInvalidArguments(UpdateBookCommand command)
        {
            UpdateBookCommandValidator validator = new();

            DomainValidationResult<UpdateBookCommand> result = await validator.ValidateAsyncGetDomainResult(command);

            result.IsFailure.Should().BeTrue();
            result.ValidationErrors.Should().HaveCount(1);
        }

        [Fact]
        public async void UpdateCommandParametersBeingAllNullChangesNothing()
        {
            IBookRepository bookRepository = Substitute.For<IBookRepository>();
            UpdateBookCommand command = new(Guid.NewGuid(), null, null, null, null, null, null);
            UpdateBookCommandValidator validator = new();
            UpdateBookCommand.UpdateBookCommandHandler handler = new(bookRepository);

            DateOnly yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
            Book book = (await Book.Create("author", "title", "description", 10, yesterday, "publisher", yesterday)).Value;

            DomainValidationResult<UpdateBookCommand> validationResult = await validator.ValidateAsyncGetDomainResult(command, CancellationToken.None);

            bookRepository.GetBookByID(Arg.Any<Guid>()).Returns(Result.Success(book));
            bookRepository.UpdateBook(Arg.Any<Guid>(), Arg.Is<Book>(b => b.Author == book.Author)).Returns(Result.Success(book));
            bookRepository.UpdateBook(Arg.Any<Guid>(), Arg.Is<Book>(b => b.Author != book.Author)).Returns(Result.Failure<Book>(null, TestReferenceHelper.TEST_ERROR));

            Result result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            validationResult.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async void UpdateCommandFailsIfGuidNotFound()
        {
            IBookRepository bookRepository = Substitute.For<IBookRepository>();
            UpdateBookCommand updateBookCommand = new(Guid.NewGuid(), null, null, null, null, null, null);
            UpdateBookCommand.UpdateBookCommandHandler handler = new(bookRepository);

            bookRepository.GetBookByID(Arg.Any<Guid>()).Returns(Result.Failure<Book>(null, TestReferenceHelper.TEST_ERROR));

            Result<Book> result = await handler.Handle(updateBookCommand, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeEquivalentTo(TestReferenceHelper.TEST_ERROR);
        }

        [Fact]
        public async void UpdateCommandCanFailAndBubbleUpError()
        {
            IBookRepository bookRepository = Substitute.For<IBookRepository>();
            UpdateBookCommand updateBookCommand = new(Guid.NewGuid(), null, null, null, null, null, null);
            UpdateBookCommand.UpdateBookCommandHandler handler = new(bookRepository);

            DateOnly yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
            Book book = (await Book.Create("author", "title", "description", 10, yesterday, "publisher", yesterday)).Value;

            bookRepository.GetBookByID(Arg.Any<Guid>()).Returns(book);
            bookRepository.UpdateBook(Arg.Any<Guid>(), Arg.Any<Book>()).Returns(Result.Failure<Book>(null, TestReferenceHelper.TEST_ERROR));

            Result<Book> result = await handler.Handle(updateBookCommand, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeEquivalentTo(TestReferenceHelper.TEST_ERROR);
        }
    }
}
