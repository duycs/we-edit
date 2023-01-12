using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Events
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventDispatcher"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        public EventDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Raises the event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event">The event.</param>
        /// <returns>Task.</returns>
        public Task RaiseEvent<T>(T @event) where T : DomainEvent
        {
            return _mediator.Publish(@event);
        }
    }
}
