using Domain.Abstract;

namespace Application.CQRS.PatronCQRS.Commands.DeletePatron
{
    public class DeletePatronCommand : ICommand
    {
        public DeletePatronCommand(Guid patronGuid)
        {
            PatronGuid = patronGuid;
        }

        public Guid PatronGuid { get; set; }

        public class DeletePatronCommandHandler : ICommandHandler<DeletePatronCommand>
        {
            IPatronRepository _patronRepository;

            public DeletePatronCommandHandler(IPatronRepository patronRepository)
            {
                _patronRepository = patronRepository;
            }

            public async Task<Result> Handle(DeletePatronCommand request, CancellationToken cancellationToken)
            {
                return await _patronRepository.DeletePatron(request.PatronGuid);
            }
        }
    }
}
