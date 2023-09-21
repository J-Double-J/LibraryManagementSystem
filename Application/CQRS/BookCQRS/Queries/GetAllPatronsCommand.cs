using Domain.Abstract;
using Domain.Entities.Patron;

namespace Application.CQRS.BookCQRS.Queries
{
    public class GetAllPatronsCommand : ICommand<IEnumerable<Patron>>
    {
        public GetAllPatronsCommand()
        {
        }

        public class GetAllPatronsCommandHandler : ICommandHandler<GetAllPatronsCommand, IEnumerable<Patron>>
        {
            IPatronRepository _patronRepository;

            public GetAllPatronsCommandHandler(IPatronRepository patronRepository)
            {
                _patronRepository = patronRepository;
            }

            public async Task<Result<IEnumerable<Patron>>> Handle(GetAllPatronsCommand request, CancellationToken cancellationToken)
            {
                return await _patronRepository.GetAllPatrons();
            }
        }
    }
}
