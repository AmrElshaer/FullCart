using Application.Common.Commands;
using Infrastructure.Common.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common
{
    public class SaveChangeCommandHandlerDecorator<T, TResult> : ICommandHandler<T, TResult> where T : ICommand<TResult>
    {
        private readonly ICommandHandler<T, TResult> _decorated;

        private readonly CartDbContext _cartDbContext;

        public SaveChangeCommandHandlerDecorator(
            ICommandHandler<T, TResult> decorated,
            CartDbContext cartDbContext)
        {
            _decorated = decorated;
            _cartDbContext = cartDbContext;
        }

        public async Task<TResult> Handle(T command, CancellationToken cancellationToken)
        {
            var result = await this._decorated.Handle(command, cancellationToken);
            await _cartDbContext.SaveChangesAsync(cancellationToken);
            return result;
        }
    }
}
