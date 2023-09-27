using Application.Interfaces;
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

            public CheckoutBookCommandHandler(IPatronRepository patronRepository,
                                              IBookRepository bookRepository,
                                              ICheckoutRepository checkoutRepository)
            {
                _patronRepository = patronRepository;
                _bookRepository = bookRepository;
                _checkoutRepository = checkoutRepository;
            }

            public async Task<Result> Handle(CheckoutBookCommand request, CancellationToken cancellationToken)
            {
                Task<Result<Patron>> patronFetchTask = _patronRepository.GetPatronByID(request.PatronGuid);
                Task<Result<Book>> bookFetchTask = _bookRepository.GetBookByID(request.BookGuid);

                await Task.WhenAll(patronFetchTask, bookFetchTask);

                Result<Patron> patronResult = patronFetchTask.Result;
                Result<Book> bookResult = bookFetchTask.Result;

                if (patronResult.IsFailure)
                {
                    return patronResult;
                }

                if (bookResult.IsFailure)
                {
                    return bookResult;
                }

                Result<Checkout> checkoutResult = await Checkout.Create(DateOnly.FromDateTime(DateTime.UtcNow),
                                                                  DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
                                                                  1,
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
