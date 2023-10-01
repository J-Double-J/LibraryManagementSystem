using Application.Interfaces;
using Domain.Abstract;
using Domain.CustomFluentValidation;
using Domain.Entities;

namespace Application.CQRS.CheckoutCQRS
{
    public class ReturnBookCommand : ICommand
    {
        public Guid BookGuid { get; init; }

        public ReturnBookCommand(Guid bookGuid)
        {
            BookGuid = bookGuid;
        }

        public class ReturnBookCommandHandler : ICommandHandler<ReturnBookCommand>
        {
            IBookRepository _bookRepository;
            ICheckoutRepository _checkoutRepository;

            public ReturnBookCommandHandler(IBookRepository bookRepository, ICheckoutRepository checkoutRepository)
            {
                _bookRepository = bookRepository;
                _checkoutRepository = checkoutRepository;
            }

            public async Task<Result> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
            {
                Result<Book> fetchedBookResult = await _bookRepository.GetBookByID(request.BookGuid);

                if(fetchedBookResult.IsFailure)
                {
                    return fetchedBookResult;
                }

                if (fetchedBookResult.Value.IsCheckedOut == false)
                {
                    ErrorCode code = ValidationErrorCodeFactory.ConstructErrorCode(LibraryValidatorType.Request, "BookNotCheckedOut");
                    return Result.Failure(new Error(code, "Book with ID `{request.BookGuid}` is not currently checked out and cannot be returned"));
                }

                return await _checkoutRepository.ReturnBook(request.BookGuid);
            }
        }
    }
}
