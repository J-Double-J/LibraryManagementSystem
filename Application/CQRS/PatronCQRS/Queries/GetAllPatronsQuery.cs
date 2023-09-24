using Domain.Abstract;
using Domain.Entities.Patron;

namespace Application.CQRS.PatronCQRS.Queries
{
    public class GetAllPatronsQuery : IQuery<IEnumerable<Patron>>
    {
        public GetAllPatronsQuery()
        {
        }

        public class GetAllPatronsQueryHandler : IQueryHandler<GetAllPatronsQuery, IEnumerable<Patron>>
        {
            IPatronRepository _patronRepository;

            public GetAllPatronsQueryHandler(IPatronRepository patronRepository)
            {
                _patronRepository = patronRepository;
            }

            public async Task<Result<IEnumerable<Patron>>> Handle(GetAllPatronsQuery request, CancellationToken cancellationToken)
            {
                return await _patronRepository.GetAllPatrons();
            }
        }
    }
}
