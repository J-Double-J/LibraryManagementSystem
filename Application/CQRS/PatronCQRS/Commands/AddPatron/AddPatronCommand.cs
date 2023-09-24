using Domain.Abstract;
using Domain.Entities.Patron;

namespace Application.CQRS.PatronCQRS.Commands.AddPatron
{
    public class AddPatronCommand : ICommand<Patron>
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public int Age { get; init; }

        public AddPatronCommand(string firstName, string lastName, int age)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
        }

        public class AddPatronCommandHandler : ICommandHandler<AddPatronCommand, Patron>
        {
            private readonly IPatronRepository _patronRepository;

            public AddPatronCommandHandler(IPatronRepository patronRepository)
            {
                _patronRepository = patronRepository;
            }

            public async Task<Result<Patron>> Handle(AddPatronCommand request, CancellationToken cancellationToken)
            {
                Result<Patron> patronResult = await Patron.Create(Guid.NewGuid(), request.FirstName, request.LastName, request.Age);

                if (patronResult.IsFailure)
                {
                    return patronResult;
                }

                return await _patronRepository.AddPatron(patronResult.Value);
            }
        }
    }
}
