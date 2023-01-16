using Domain;
using Infrastructure.Commands;
using Infrastructure.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Operations.Actions.GetStaffAction
{
    public class GetStaffAction : OperationBase
    {
        public GetStaffAction(Operation operation, InvokeResult invokeResult, ILogger logger, ICommandDispatcher commandDispatcher) : base(operation, invokeResult, logger, commandDispatcher)
        {
        }

        public override OperationBase PreProcessing()
        {
            throw new NotImplementedException();
        }

        public override Task<OperationBase> Processing()
        {
            throw new NotImplementedException();
        }

        public override OperationBase PostProcessing()
        {
            throw new NotImplementedException();
        }
      
    }
}
