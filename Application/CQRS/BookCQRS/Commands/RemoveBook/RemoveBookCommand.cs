using Domain.Abstract;

namespace Application.CQRS.BookCQRS.Commands.RemoveBook
{
    public class RemoveBookCommand : ICommand
    {
        public RemoveBookCommand(Guid guid)
        {
            Guid = guid;
        }

        public Guid Guid { get; init; }

        public class RemoveBookCommandHandler : ICommandHandler<RemoveBookCommand>
        {
            private readonly IBookRepository _repository;
            public RemoveBookCommandHandler(IBookRepository bookRepository)
            {
                _repository = bookRepository;
            }

            public async Task<Result> Handle(RemoveBookCommand request, CancellationToken cancellationToken)
            {
                Result result = await _repository.DeleteBook(request.Guid);

                return result;
            }
        }
    }
}
