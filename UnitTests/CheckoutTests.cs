using Domain.Abstract;
using Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace UnitTests
{
    public class CheckoutTests
    {
        [Fact]
        public async void Checkout_CanBeSuccessfullyCreated()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            Patron patron = (await Patron.Create(Guid.NewGuid(), "Tester", "Tester", 18)).Value;
            Book book = (await Book.Create("Author", "Title", "Description", 10, today, "publisher", today)).Value;
            Result<Checkout> checkout = await Checkout.Create(today, today.AddDays(7), 1, false, patron, book);

            checkout.IsSuccess.Should().BeTrue();
            checkout.Value.PatronId.Should().Be(patron.Id);
            checkout.Value.BookId.Should().Be(book.Id);
            checkout.Value.DateReturned.Should().BeNull();
        }

        [Fact]
        public async void Checkout_FailsIfAllParametersInvalid()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            Patron patron = (await Patron.Create(Guid.NewGuid(), "Tester", "Tester", 18)).Value;
            Book book = (await Book.Create("Author", "Title", "Description", 10, today, "publisher", today)).Value;
            Result<Checkout> checkout = await Checkout.Create(today.AddDays(1), today, -1, true, patron, book);

            checkout.IsFailure.Should().BeTrue();
            checkout.Should().BeOfType<DomainValidationResult<Checkout>>()
                .Which.ValidationErrors.Should().HaveCount(4);
        }

        [Fact]
        public async void Checkout_CanReturnBook()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            Patron patron = (await Patron.Create(Guid.NewGuid(), "Tester", "Tester", 18)).Value;
            Book book = (await Book.Create("Author", "Title", "Description", 10, today, "publisher", today)).Value;
            Result<Checkout> checkoutResult = await Checkout.Create(today, today.AddDays(7), 1, false, patron, book);

            Checkout checkout = checkoutResult.Value;
            checkout.ReturnBook();

            checkout.IsReturned.Should().BeTrue();
            checkout.Book.IsCheckedOut.Should().BeFalse();
            checkout.DateReturned.Should().Be(today);
        }

        [Fact]
        public async void Checkout_CanRenew()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            Patron patron = (await Patron.Create(Guid.NewGuid(), "Tester", "Tester", 18)).Value;
            Book book = (await Book.Create("Author", "Title", "Description", 10, today, "publisher", today)).Value;
            Result<Checkout> checkoutResult = await Checkout.Create(today, today.AddDays(7), 1, false, patron, book);

            Checkout checkout = checkoutResult.Value;
            Result<bool> renewalResult = checkout.TryRenew(7);

            renewalResult.IsSuccess.Should().BeTrue().And.Subject!.Value.Should().BeTrue();
            checkout.RenewalsLeft.Equals(0);
            checkout.DueDate.Should().Be(today.AddDays(14));
        }

        [Fact]
        public async void Checkout_CannotRenewWhenNoRenewalsLeft()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            Patron patron = (await Patron.Create(Guid.NewGuid(), "Tester", "Tester", 18)).Value;
            Book book = (await Book.Create("Author", "Title", "Description", 10, today, "publisher", today)).Value;
            Result<Checkout> checkoutResult = await Checkout.Create(today, today.AddDays(7), 0, false, patron, book);

            Checkout checkout = checkoutResult.Value;
            Result<bool> renewalResult = checkout.TryRenew(7);

            // The renewal process worked, but you do not get to successfully renew.
            renewalResult.IsSuccess.Should().BeTrue();
            checkout.RenewalsLeft.Equals(0);
            checkout.DueDate.Equals(today.AddDays(7));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async void Checkout_CannotAddZeroOrLessDaysToDueDate_OnRenewal(int invalidDaysToAdd)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            Patron patron = (await Patron.Create(Guid.NewGuid(), "Tester", "Tester", 18)).Value;
            Book book = (await Book.Create("Author", "Title", "Description", 10, today, "publisher", today)).Value;
            Result<Checkout> checkoutResult = await Checkout.Create(today, today.AddDays(7), 1, false, patron, book);

            Checkout checkout = checkoutResult.Value;
            Result<bool> renewalResult = checkout.TryRenew(invalidDaysToAdd);

            renewalResult.IsFailure.Should().BeTrue();
            checkout.RenewalsLeft.Equals(1);
            checkout.DueDate.Equals(today.AddDays(7));
        }
    }
}
