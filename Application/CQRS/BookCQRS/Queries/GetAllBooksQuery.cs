using Domain.Abstract;
using Domain.Entities;

namespace Application.CQRS.BookCQRS.Queries
{
    public sealed class GetAllBooksQuery : IQuery<IEnumerable<Book>>
    {
        public sealed class GetAllBooksQueryHandler : IQueryHandler<GetAllBooksQuery, IEnumerable<Book>>
        {
            private readonly IBookRepository _bookRepository;
            public GetAllBooksQueryHandler(IBookRepository bookRepository)
            {
                _bookRepository = bookRepository;
            }

            public async Task<Result<IEnumerable<Book>>> Handle(GetAllBooksQuery query, CancellationToken cancellationToken)
            {
                IEnumerable<Book> books = await _bookRepository.GetAllBooks();
                return Result.Success(books);
            }
        }
    }
}
