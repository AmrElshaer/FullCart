using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Common.Commands
{
    public interface ICommand : IRequest
    {
    }

    public interface ICommand<out TResult> : IRequest<TResult>
    {
    }
    public interface ICommandHandler<in TCommand> :
      IRequestHandler<TCommand> where TCommand : ICommand
    {

    }

    public interface ICommandHandler<in TCommand, TResult> :
        IRequestHandler<TCommand, TResult> where TCommand : ICommand<TResult>
    {

    }
}
