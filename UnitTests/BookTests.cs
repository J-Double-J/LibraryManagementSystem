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

            Result<Book> bookResult = await Book.Create("Author", "Title", "Description", 1, tomorrow, "Publisher", tomorrow);

            bookResult.Should().BeOfType<DomainValidationResult<Book>>()
                .Which.ValidationErrors.Should().HaveCount(2);
        }

        [Fact]
        public async void CompletelyInvalidBook_ReturnsAllValidationErrors()
        {
            DateTime today = DateTime.UtcNow;
            DateOnly tomorrow = DateOnly.FromDateTime(today.AddDays(1));

            Result<Book> bookResult = await Book.Create("", "", "", -1, tomorrow, "", tomorrow);

            bookResult.Should().BeOfType<DomainValidationResult<Book>>()
                .Which.ValidationErrors.Should().HaveCount(7);
        }

        [Fact]
        public async void BookCanBeConstructedCorrectly()
        {
            DateOnly yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

            Result<Book> bookResult = await Book.Create("Author", "Title", "Description", 10, yesterday, "publisher", yesterday);

            bookResult.IsSuccess.Should().BeTrue();
            bookResult.Value.GetType().Should().Be(typeof(Book));
        }

        [Fact]
        public async void CopyToBookCopiesCorrectly()
        {
            DateOnly yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
            DateOnly evenEarlier = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2));

            Book bookOne = (await Book.Create("author", "One", "description", 10, yesterday, "publisher", yesterday)).Value!;
            Book bookTwo = (await Book.Create("a", "Two", "d", 1, evenEarlier, "p", evenEarlier)).Value!;

            bookOne.CopyTo(bookTwo);

            bookOne.Author.Should().BeEquivalentTo(bookTwo.Author);
            bookOne.Title.Should().BeEquivalentTo(bookTwo.Title);
            bookOne.Description.Should().BeEquivalentTo(bookTwo.Description);
            bookOne.Pages.Should().Be(bookTwo.Pages);
            bookOne.DatePublished.Should().Be(bookTwo.DatePublished);
            bookOne.Publisher.Should().BeEquivalentTo(bookOne.Publisher);
            bookOne.DateRecieved.Should().Be(bookTwo.DateRecieved);
        }
    }
}