﻿using Domain;
using Infrastructure.Commands;
using Infrastructure.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Operations
{
    public class NotImplementOperation : OperationBase
    {
        public NotImplementOperation(Operation operation, InvokeResult invokeResult, ILogger logger, ICommandDispatcher commandDispatcher) : base(operation, invokeResult, logger, commandDispatcher)
        {
        }

        public override OperationBase PostProcessing()
        {
            throw new NotImplementedException();
        }

        public override OperationBase PreProcessing()
        {
            throw new NotImplementedException();
        }

        public override Task<OperationBase> Processing()
        {
            throw new NotImplementedException();
        }
    }
}
