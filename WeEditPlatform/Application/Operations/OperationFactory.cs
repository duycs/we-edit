using Application.Operations.Actions.AssignAction;
using Application.Operations.Actions.GetJobStepAction;
using Application.Operations.Actions.GetStaffAction;
using Domain;
using Infrastructure.Commands;
using Infrastructure.Models;
using Microsoft.Extensions.Logging;

namespace Application.Operations
{
    public class OperationFactory
    {
        private Dictionary<ExecutionName, Func<OperationBase>> _operationMapper;
        private readonly Operation _operation;

        public OperationFactory(Operation operation, InvokeResult invokeResult, ILogger logger, ICommandDispatcher commandDispatcher)
        {
            _operation = operation;
            _operationMapper = new Dictionary<ExecutionName, Func<OperationBase>>();
            _operationMapper.Add(ExecutionName.NotImplement, () => { return new NotImplementOperation(operation, invokeResult, logger, commandDispatcher); });
            _operationMapper.Add(ExecutionName.AssignAction, () => { return new AssignAction(operation, invokeResult, logger, commandDispatcher); });
            _operationMapper.Add(ExecutionName.GetJobStepAction, () => { return new GetJobStepAction(operation, invokeResult, logger, commandDispatcher); });
            _operationMapper.Add(ExecutionName.GetStaffAction, () => { return new GetStaffAction(operation, invokeResult, logger, commandDispatcher); });
        }

        public OperationBase? GetOperationBasedOnOperation()
        {
            try
            {
                var executionName = (ExecutionName)Enum.Parse(typeof(ExecutionName), _operation.ExecutionName ?? ExecutionName.NotImplement.ToString(), true);
                return _operationMapper[executionName]();
            }
            catch (Exception ex)
            {
                // Operation not found
                return null;
            }
        }
    }
}
