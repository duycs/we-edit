﻿using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Commands
{
    public interface ICommandDispatcher
    {
        Task Send<T>(T command) where T : Command;
        Task<InvokeResult> SendGetResponse<T>(T command) where T : CommandResponse<InvokeResult>;
    }
}
