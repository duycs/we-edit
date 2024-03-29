﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Events
{
    public interface IEventDispatcher
    {
        Task RaiseEvent<T>(T @event) where T : DomainEvent;
    }
}
