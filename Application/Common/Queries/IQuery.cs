using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Queries
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {

    }
    public interface IQueryHandler<in TQuery, TResult> :
     IRequestHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {

    }
}
