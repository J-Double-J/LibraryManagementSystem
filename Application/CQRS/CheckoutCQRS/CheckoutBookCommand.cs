using Application.Interfaces;
using Application.Interfaces.Configuration;
using Domain.Abstract;
using Domain.Entities;

namespace Application.CQRS.CheckoutCQRS
{
    public class CheckoutBookCommand : ICommand
    {
        public Guid PatronGuid { get; set; }
        public Guid BookGuid { get; set; }

        public CheckoutBookCommand(Guid patronGuid, Guid bookGuid)
        {
            PatronGuid = patronGuid;
            BookGuid = bookGuid;
        }

        public class CheckoutBookCommandHandler : ICommandHandler<CheckoutBookCommand>
        {
            IPatronRepository _patronRepository;
            IBookRepository _bookRepository;
            ICheckoutRepository _checkoutRepository;
            ILibraryManagementConfiguration _configuration;

            public CheckoutBookCommandHandler(IPatronRepository patronRepository,
                                              IBookRepository bookRepository,
                                              ICheckoutRepository checkoutRepository,
                                              ILibraryManagementConfiguration configuration)
            {
                _patronRepository = patronRepository;
                _bookRepository = bookRepository;
                _checkoutRepository = checkoutRepository;
                _configuration = configuration;
            }

            public async Task<Result> Handle(CheckoutBookCommand request, CancellationToken cancellationToken)
            {
                Result<Patron> patronResult = await _patronRepository.GetPatronByID(request.PatronGuid);
                Result<Book> bookResult = await _bookRepository.GetBookByID(request.BookGuid);

                if (patronResult.IsFailure)
                {
                    return patronResult;
                }

                if (bookResult.IsFailure)
                {
                    return bookResult;
                }

                Result<Checkout> checkoutResult = await Checkout.Create(DateOnly.FromDateTime(DateTime.UtcNow),
                                                                  DateOnly.FromDateTime(DateTime.UtcNow.AddDays(_configuration.RenewalPolicy.RenewalPeriodInDays)),
                                                                  _configuration.RenewalPolicy.MaximumRenewals,
                                                                  false,
                                                                  patronResult.Value,
                                                                  bookResult.Value);

                if (checkoutResult.IsFailure)
                {
                    return checkoutResult;
                }

                Result transactionResult = await _checkoutRepository.AddCheckoutTransaction(checkoutResult.Value);

                if(transactionResult.IsSuccess)
                {
                    Result updateBookToCheckout = await _bookRepository.CheckoutBook(bookResult.Value);
                    if(updateBookToCheckout.IsFailure)
                    {
                        return updateBookToCheckout;
                    }
                }

                return transactionResult;
            }
        }
    }
}
