using Domain.Abstract;
using Domain.Entities;

namespace Application.CQRS.PatronCQRS.Queries
{
    public class GetPatronByIDQuery : IQuery<Patron>
    {
        public Guid PatronGuid { get; init; }

        public GetPatronByIDQuery(Guid patronGuid)
        {
            PatronGuid = patronGuid;
        }

        public class GetPatronByIDQueryHandler : IQueryHandler<GetPatronByIDQuery, Patron>
        {
            IPatronRepository _patronRepository;

            public GetPatronByIDQueryHandler(IPatronRepository patronRepository)
            {
                _patronRepository = patronRepository;
            }

            public async Task<Result<Patron>> Handle(GetPatronByIDQuery request, CancellationToken cancellationToken)
            {
                return await _patronRepository.GetPatronByID(request.PatronGuid);
            }
        }
    }
}
