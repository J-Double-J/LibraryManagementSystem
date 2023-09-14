using Domain.Abstract;
using MediatR;

namespace Application.CQRS
{
    public interface ICommand : IRequest<Result>, ICommandBase
    {
    }

    public interface ICommand<TResponse> : IRequest<Result<TResponse>>, ICommandBase
    {
    }

    public interface ICommandBase
    {
    }
}
