using Domain.Entities;
using MediatR;

namespace Application.CQRS.BookCQRS.Queries
{
    public class GetAllBooksQuery : IRequest<IEnumerable<Book>>
    {
        public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, IEnumerable<Book>>
        {
            public async Task<IEnumerable<Book>> Handle(GetAllBooksQuery query, CancellationToken cancellationToken)
            {
                return new List<Book> { Book.Create(new Guid(), "Placeholder Author",
                        "Placeholder Title",
                        "Placeholder Description",
                        1,
                        DateOnly.FromDateTime(DateTime.Now),
                        "Placeholder Publisher",
                        DateOnly.FromDateTime(DateTime.Now)) };
            }
        }
    }
}
