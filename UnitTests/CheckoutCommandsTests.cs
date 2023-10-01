using Application;
using Application.CQRS.CheckoutCQRS;
using Application.Interfaces;
using Application.Interfaces.Configuration;
using Domain.Abstract;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Configuration.Objects;
using NSubstitute;

namespace UnitTests
{
    public class CheckoutCommandsTests
    {
        [Fact]
        public async void BookCanBeCheckedOut()
        {
            // Create Book and Patron
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            Patron patron = (await Patron.Create(Guid.NewGuid(), "Tester", "Tester", 18)).Value;
            Book book = (await Book.Create("Author", "Title", "Description", 10, today, "publisher", today)).Value;
            RenewalPolicy renewalPolicy = new() { MaximumRenewals = 2, RenewalPeriodInDays = 7 };

            // Mock repositories
            IPatronRepository patronRepository = Substitute.For<IPatronRepository>();
            patronRepository.GetPatronByID(Arg.Is<Guid>(id => id == patron.Id)).Returns(patron);

            IBookRepository bookRepository = Substitute.For<IBookRepository>();
            bookRepository.GetBookByID(Arg.Is<Guid>(id => id == book.Id)).Returns(book);
            bookRepository.CheckoutBook(Arg.Is<Book>(b => b == book)).Returns(Result.Success()).AndDoes(x => { book.IsCheckedOut = true; });
          
            ICheckoutRepository checkoutRepository = Substitute.For<ICheckoutRepository>();
            checkoutRepository
                .AddCheckoutTransaction(Arg.Is<Checkout>(checkout =>
                    checkout.RenewalsLeft == renewalPolicy.MaximumRenewals &&
                    checkout.DueDate == today.AddDays(7)
                ))
                .Returns(Result.Success());
            checkoutRepository
                .AddCheckoutTransaction(Arg.Is<Checkout>(checkout =>
                    checkout.RenewalsLeft != renewalPolicy.MaximumRenewals ||
                    checkout.DueDate != today.AddDays(7)
                ))
                .Returns(Result.Failure<Checkout>(null, TestReferenceHelper.TEST_ERROR));

            ILibraryManagementConfiguration configuration = Substitute.For<ILibraryManagementConfiguration>();
            configuration.RenewalPolicy.Returns(renewalPolicy);

            CheckoutBookCommand command = new(patron.Id, book.Id);
            CheckoutBookCommand.CheckoutBookCommandHandler handler = new(patronRepository, bookRepository, checkoutRepository, configuration);

            Result result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            book.IsCheckedOut.Should().BeTrue();
        }

        [Fact]
        public async void CheckoutFails_IfPatronRepositoryFails()
        {
            // Create Book and Patron
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            Patron patron = (await Patron.Create(Guid.NewGuid(), "Tester", "Tester", 18)).Value;
            Book book = (await Book.Create("Author", "Title", "Description", 10, today, "publisher", today)).Value;
            
            // Mock Repositories
            IPatronRepository patronRepository = Substitute.For<IPatronRepository>();
            patronRepository.GetPatronByID(Arg.Is<Guid>(id => id == patron.Id)).Returns(Result.Failure<Patron>(null, TestReferenceHelper.TEST_ERROR));

            IBookRepository bookRepository = Substitute.For<IBookRepository>();
            ICheckoutRepository checkoutRepository = Substitute.For<ICheckoutRepository>();
            ILibraryManagementConfiguration configuration = Substitute.For<ILibraryManagementConfiguration>();

            CheckoutBookCommand command = new(patron.Id, book.Id);
            CheckoutBookCommand.CheckoutBookCommandHandler handler = new(patronRepository, bookRepository, checkoutRepository, configuration);

            Result result = await handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Should().BeOfType(typeof(Result<Patron>));
        }

        [Fact]
        public async void CheckoutFails_IfBookRepositoryFails()
        {
            // Create Book and Patron
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            Patron patron = (await Patron.Create(Guid.NewGuid(), "Tester", "Tester", 18)).Value;
            Book book = (await Book.Create("Author", "Title", "Description", 10, today, "publisher", today)).Value;

            // Mock repositories
            IPatronRepository patronRepository = Substitute.For<IPatronRepository>();
            patronRepository.GetPatronByID(Arg.Is<Guid>(id => id == patron.Id)).Returns(patron);

            IBookRepository bookRepository = Substitute.For<IBookRepository>();
            bookRepository.GetBookByID(Arg.Any<Guid>()).Returns(Result.Failure<Book>(null, TestReferenceHelper.TEST_ERROR));

            ICheckoutRepository checkoutRepository = Substitute.For<ICheckoutRepository>();
            ILibraryManagementConfiguration configuration = Substitute.For<ILibraryManagementConfiguration>();

            CheckoutBookCommand command = new(patron.Id, book.Id);
            CheckoutBookCommand.CheckoutBookCommandHandler handler = new(patronRepository, bookRepository, checkoutRepository, configuration);

            Result result = await handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Should().BeOfType(typeof(Result<Book>));
        }

        [Fact]
        public async void CheckoutFails_IfAddingCheckoutToRepositoryFails()
        {
            // Create Book and Patron
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            Patron patron = (await Patron.Create(Guid.NewGuid(), "Tester", "Tester", 18)).Value;
            Book book = (await Book.Create("Author", "Title", "Description", 10, today, "publisher", today)).Value;
            RenewalPolicy renewalPolicy = new() { MaximumRenewals = 2, RenewalPeriodInDays = 7 };

            // Mock repositories
            IPatronRepository patronRepository = Substitute.For<IPatronRepository>();
            patronRepository.GetPatronByID(Arg.Is<Guid>(id => id == patron.Id)).Returns(patron);

            IBookRepository bookRepository = Substitute.For<IBookRepository>();
            bookRepository.GetBookByID(Arg.Any<Guid>()).Returns(book);

            ICheckoutRepository checkoutRepository = Substitute.For<ICheckoutRepository>();
            checkoutRepository.AddCheckoutTransaction(Arg.Any<Checkout>()).Returns(Result.Failure(TestReferenceHelper.TEST_ERROR));

            ILibraryManagementConfiguration configuration = Substitute.For<ILibraryManagementConfiguration>();
            configuration.RenewalPolicy.Returns(renewalPolicy);

            CheckoutBookCommand command = new(patron.Id, book.Id);
            CheckoutBookCommand.CheckoutBookCommandHandler handler = new(patronRepository, bookRepository, checkoutRepository, configuration);

            Result result = await handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Should().BeOfType(typeof(Result));
        }

        [Fact]
        public async void CheckoutFails_IfCheckingBookOutInRepositoryFails()
        {
            // Create Book and Patron
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            Patron patron = (await Patron.Create(Guid.NewGuid(), "Tester", "Tester", 18)).Value;
            Book book = (await Book.Create("Author", "Title", "Description", 10, today, "publisher", today)).Value;
            RenewalPolicy renewalPolicy = new() { MaximumRenewals = 2, RenewalPeriodInDays = 7 };

            // Mock repositories
            IPatronRepository patronRepository = Substitute.For<IPatronRepository>();
            patronRepository.GetPatronByID(Arg.Is<Guid>(id => id == patron.Id)).Returns(patron);

            IBookRepository bookRepository = Substitute.For<IBookRepository>();
            bookRepository.GetBookByID(Arg.Any<Guid>()).Returns(book);
            bookRepository.CheckoutBook(Arg.Is<Book>(b => b == book)).Returns(Result.Failure(TestReferenceHelper.TEST_ERROR));


            ICheckoutRepository checkoutRepository = Substitute.For<ICheckoutRepository>();
            checkoutRepository.AddCheckoutTransaction(Arg.Any<Checkout>()).Returns(Result.Success());

            ILibraryManagementConfiguration configuration = Substitute.For<ILibraryManagementConfiguration>();
            configuration.RenewalPolicy.Returns(renewalPolicy);

            CheckoutBookCommand command = new(patron.Id, book.Id);
            CheckoutBookCommand.CheckoutBookCommandHandler handler = new(patronRepository, bookRepository, checkoutRepository, configuration);

            Result result = await handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Should().BeOfType(typeof(Result));
        }
    }
}
