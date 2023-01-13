using Infrastructure.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Commands
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDispatcher"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        public CommandDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Sends the specified command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task Send<T>(T command) where T : Command
        {
            return _mediator.Send(command);
        }

        async Task<InvokeResult> ICommandDispatcher.SendGetResponse<T>(T command)
        {
            return await _mediator.Send(command);
        }
    }
}
