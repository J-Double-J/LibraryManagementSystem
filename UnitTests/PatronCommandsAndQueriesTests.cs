using Application;
using Application.CQRS.PatronCQRS.Commands.AddPatron;
using Application.CQRS.PatronCQRS.Commands.DeletePatron;
using Application.CQRS.PatronCQRS.Queries;
using Domain.Abstract;
using Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace UnitTests
{
    public class PatronCommandsAndQueriesTests
    {
        [Fact]
        public async void AddPatronCommand_CanBeSuccessful()
        {
            Patron patron = (await Patron.Create(Guid.NewGuid(), "John", "Smith", 22)).Value;
            
            IPatronRepository patronRepository = Substitute.For<IPatronRepository>();
            patronRepository.AddPatron(Arg.Any<Patron>()).Returns(callInfo => callInfo.Args()[0] as Patron);

            AddPatronCommand command = new("John", "Smith", 22);
            AddPatronCommand.AddPatronCommandHandler handler = new(patronRepository);

            Result<Patron> result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.FirstName.Should().BeEquivalentTo(patron.FirstName);
            result.Value.LastName.Should().BeEquivalentTo(patron.LastName);
            result.Value.Age.Should().Be(patron.Age);
        }

        [Fact]
        public async void AddPatronCommand_CanFail_IfInvalidPatronCreated()
        {
            IPatronRepository patronRepository = Substitute.For<IPatronRepository>();
            patronRepository.AddPatron(Arg.Any<Patron>()).Returns(callInfo => callInfo.Args()[0] as Patron);

            AddPatronCommand command = new("Josh", "Jacobs", -1);
            AddPatronCommand.AddPatronCommandHandler handler = new(patronRepository);

            Result<Patron> result = await handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public async void DeletePatronCommand_CanSucceed()
        {
            IPatronRepository patronRepository = Substitute.For<IPatronRepository>();
            patronRepository.DeletePatron(Arg.Any<Guid>()).Returns(Result.Success());

            DeletePatronCommand command = new(Guid.NewGuid());
            DeletePatronCommand.DeletePatronCommandHandler handler = new(patronRepository);

            Result result = await handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async void DeletePatronCommand_CanFail()
        {
            IPatronRepository patronRepository = Substitute.For<IPatronRepository>();
            patronRepository.DeletePatron(Arg.Any<Guid>()).Returns(Result.Failure(TestReferenceHelper.TEST_ERROR));

            DeletePatronCommand command = new(Guid.NewGuid());
            DeletePatronCommand.DeletePatronCommandHandler handler = new(patronRepository);

            Result result = await handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public async void GetAllPatronsQuery_CanSucceed()
        {
            IPatronRepository patronRepository = Substitute.For<IPatronRepository>();
            patronRepository.GetAllPatrons().Returns(Result.Success(Enumerable.Empty<Patron>()));

            GetAllPatronsQuery.GetAllPatronsQueryHandler handler = new(patronRepository);

            Result<IEnumerable<Patron>> result = await handler.Handle(new(), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Count().Should().Be(0);
        }

        [Fact]
        public async void GetAllPatronsQuery_CanFail()
        {
            IPatronRepository patronRepository = Substitute.For<IPatronRepository>();
            patronRepository.GetAllPatrons().Returns(Result.Failure<IEnumerable<Patron>>(null, TestReferenceHelper.TEST_ERROR));

            GetAllPatronsQuery.GetAllPatronsQueryHandler handler = new(patronRepository);

            Result<IEnumerable<Patron>> result = await handler.Handle(new(), CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public async void GetPatronByID_CanSucceed()
        {
            Patron patron = (await Patron.Create(Guid.NewGuid(), "John", "Smith", 22)).Value;

            IPatronRepository patronRepository = Substitute.For<IPatronRepository>();
            patronRepository.GetPatronByID(Arg.Is<Guid>(guid => guid == patron.Id)).Returns(patron);

            GetPatronByIDQuery query = new(patron.Id);
            GetPatronByIDQuery.GetPatronByIDQueryHandler handler = new(patronRepository);

            Result<Patron> result = await handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(patron);
        }

        [Fact]
        public async void GetPatronByID_CanFail()
        {
            IPatronRepository patronRepository = Substitute.For<IPatronRepository>();
            patronRepository.GetPatronByID(Arg.Any<Guid>()).Returns(Result.Failure<Patron>(null, TestReferenceHelper.TEST_ERROR));

            GetPatronByIDQuery query = new(Guid.NewGuid());
            GetPatronByIDQuery.GetPatronByIDQueryHandler handler = new(patronRepository);

            Result<Patron> result = await handler.Handle(query, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }
    }
}
