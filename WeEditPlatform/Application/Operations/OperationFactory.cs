using Application.Operations.Actions.AssignAction;
using Domain;
using Infrastructure.Commands;
using Microsoft.Extensions.Logging;

namespace Application.Operations
{
    public class OperationFactory
    {
        private Dictionary<ExecutionName, Func<OperationBase>> _operationMapper;
        private readonly Operation _operation;

        public OperationFactory(Operation operation, ILogger logger, ICommandDispatcher commandDispatcher)
        {
            _operation = operation;
            _operationMapper = new Dictionary<ExecutionName, Func<OperationBase>>();
            _operationMapper.Add(ExecutionName.NotImplement, () => { return new NotImplementOperation(); });
            _operationMapper.Add(ExecutionName.FilterStaff, () => { return new NotImplementOperation(); });
            _operationMapper.Add(ExecutionName.FilterJobStep, () => { return new NotImplementOperation(); });
            _operationMapper.Add(ExecutionName.AssignAction, () => { return new AssignAction(operation, logger, commandDispatcher); });
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
