using Domain.Abstract;
using Domain.Entities;

namespace Application.CQRS.BookCQRS.Queries
{
    public sealed class GetBookByIDQuery : IQuery<Book>
    {
        public Guid Guid { get; set; }
        public GetBookByIDQuery(Guid guid)
        {
            Guid = guid;
        }

        public sealed class GetBookByIDQueryHandler : IQueryHandler<GetBookByIDQuery, Book>
        {
            private readonly IBookRepository _bookRepository;
            public GetBookByIDQueryHandler(IBookRepository bookRepository)
            {
                _bookRepository = bookRepository;
            }

            public async Task<Result<Book>> Handle(GetBookByIDQuery request, CancellationToken cancellationToken)
            {
                return await _bookRepository.GetBookByID(request.Guid);
            }
        }
    }
}
