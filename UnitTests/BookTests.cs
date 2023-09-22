using Domain.Abstract;
using Domain.Entities;
using FluentAssertions;

namespace UnitTests
{
    public class BookTests
    {        
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async void Book_CannotHave_NegativeOrZeroPages(int pages)
        {
            DateOnly yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
            Result<Book> bookResult = await Book.Create("Author", "Title", "Description", pages, yesterday, "Publisher", yesterday);

            bookResult.Should().BeOfType<DomainValidationResult<Book>>()
                .Which.ValidationErrors.Should().HaveCount(1)
                .And.Subject.First().Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async void Book_CannotHave_EmptyAuthor()
        {
            DateOnly yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
            Result<Book> bookResult = await Book.Create("", "Title", "Description", 1, yesterday, "Publisher", yesterday);

            bookResult.Should().BeOfType<DomainValidationResult<Book>>()
                .Which.ValidationErrors.Should().HaveCount(1)
                .And.Subject.First().Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async void Book_CannotHave_EmptyDescription()
        {
            DateOnly yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
            Result<Book> bookResult = await Book.Create("Author", "Title", "", 1, yesterday, "Publisher", yesterday);

            bookResult.Should().BeOfType<DomainValidationResult<Book>>()
                .Which.ValidationErrors.Should().HaveCount(1)
                .And.Subject.First().Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async void Book_CannotHave_LongTitle()
        {
            string longTitle = new('a', Book.MAX_LENGTH_TITLE + 1);
            DateOnly yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
            Result<Book> bookResult = await Book.Create("Author", longTitle, "Description", 1, yesterday, "Publisher", yesterday);

            bookResult.Should().BeOfType<DomainValidationResult<Book>>()
                .Which.ValidationErrors.Should().HaveCount(1)
                .And.Subject.First().Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async void Book_CannotHave_EmptyTitle()
        {
            DateOnly yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
            Result<Book> bookResult = await Book.Create("Author", "", "Description", 1, yesterday, "Publisher", yesterday);

            bookResult.Should().BeOfType<DomainValidationResult<Book>>()
                .Which.ValidationErrors.Should().HaveCount(1)
                .And.Subject.First().Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async void Book_CannotHave_LongPublisherName()
        {
            string longPublisher = new('a', Book.MAX_LENGTH_PUBLISHER + 1);
            DateOnly yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
            Result<Book> bookResult = await Book.Create("Author", "Title", "Description", 1, yesterday, longPublisher, yesterday);

            bookResult.Should().BeOfType<DomainValidationResult<Book>>()
                .Which.ValidationErrors.Should().HaveCount(1)
                .And.Subject.First().Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async void Book_CannotHave_EmptyPublisher()
        {
            DateOnly yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
            Result<Book> bookResult = await Book.Create("Author", "Title", "Description", 1, yesterday, "", yesterday);

            bookResult.Should().BeOfType<DomainValidationResult<Book>>()
                .Which.ValidationErrors.Should().HaveCount(1)
                .And.Subject.First().Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async void Book_CannotHave_PublishedDateAfterToday()
        {
            DateTime today = DateTime.UtcNow;
            DateOnly tomorrow = DateOnly.FromDateTime(today.AddDays(1));
            DateOnly yesterday = DateOnly.FromDateTime(today.AddDays(-1));

            Result<Book> bookResult = await Book.Create("Author", "Title", "Description", 1, tomorrow, "Publisher", yesterday);

            bookResult.Should().BeOfType<DomainValidationResult<Book>>()
                .Which.ValidationErrors.Should().HaveCount(1)
                .And.Subject.First().Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async void Book_CannotBeRecieved_AfterToday()
        {
            DateTime today = DateTime.UtcNow;
            DateOnly tomorrow = DateOnly.FromDateTime(today.AddDays(1));
            DateOnly yesterday = DateOnly.FromDateTime(today.AddDays(-1));

            Result<Book> bookResult = await Book.Create("Author", "Title", "Description", 1, yesterday, "Publisher", tomorrow);

            bookResult.Should().BeOfType<DomainValidationResult<Book>>()
                .Which.ValidationErrors.Should().HaveCount(1)
                .And.Subject.First().Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async void Book_CannotHaveMinimumDate_ForPublishedAndRecievedDate()
        {
            DateTime today = DateTime.UtcNow;
            DateOnly tomorrow = DateOnly.FromDateTime(today.AddDays(1));
            DateOnly yesterday = DateOnly.FromDateTime(today.AddDays(-1));

            Result<Book> bookResult = await Book.Create("Author", "Title", "Description", 1, tomorrow, "Publisher", tomorrow);

            bookResult.Should().BeOfType<DomainValidationResult<Book>>()
                .Which.ValidationErrors.Should().HaveCount(2);
        }
    }
}