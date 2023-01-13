using Application.Models;
using Application.Operations;
using Application.Queries;
using Domain;
using Infrastructure.Commands;
using Infrastructure.Models;
using Infrastructure.Repository;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Application.Services
{
    public class OperationService : IOperationService
    {
        private readonly ILogger<OperationService> _logger;
        private readonly IRepositoryService _repositoryService;
        private readonly IRouteService _routeService;
        private readonly ICommandDispatcher _commandDispatcher;

        public OperationService(ILogger<OperationService> logger,
            IRepositoryService repositoryService,
            ISettingService settingService,
            IRouteService routeService,
            ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _repositoryService = repositoryService;
            _routeService = routeService;
            _commandDispatcher = commandDispatcher;
        }

        public Operation CreateOperation(CreateOperationVM request)
        {
            var flow = _repositoryService.Find<Flow>(request.FlowId);
            if (flow == null)
            {
                throw new Exception($"Flow {request.FlowId} not found");
            }

            var operation = _repositoryService.Add(Operation.Create(flow, request.Type, request.Name, request.Description,
                                                    request.ExecutionName, request.FirstRoute, request.Settings));
            _repositoryService.SaveChanges();

            return operation;
        }

        public Operation GetOperation(int id, bool isInclude)
        {
            return _repositoryService.Find<Operation>(id, new OperationSpecification(isInclude));
        }

        public void RemoveOperation(int id)
        {
            var operation = _repositoryService.Find<Operation>(id);

            if (operation == null)
            {
                throw new Exception($"Operation {id} not found");
            }

            _repositoryService.Delete(operation);
            _repositoryService.SaveChanges();
        }

        public Operation UpdateOperation(UpdateOperationVM request)
        {
            var operation = _repositoryService.Find<Operation>(request.Id);

            if (operation == null)
            {
                throw new Exception($"Operation {request.Id} not found");
            }

            var operationUpdated = operation.Update(request.Type, request.Name, request.Description,
                            request.ExecutionName, request.FirstRoute, request.Settings);

            _repositoryService.Update(operationUpdated);
            _repositoryService.SaveChanges();

            return operationUpdated;
        }

        public void SetActiveOperation(int id)
        {
            var operation = _repositoryService.Find<Operation>(id);

            if (operation == null)
            {
                throw new Exception($"Operation {id} not found");
            }

            operation.SetActive();
            _repositoryService.Update(operation);
            _repositoryService.SaveChanges();
        }

        public async Task<InvokeResult> Invoke(int id)
        {
            var message = new StringBuilder();
            try
            {
                var operation = _repositoryService.Find(id, new OperationSpecification(true));

                if (operation == null)
                {
                    message.AppendLine($"Operation {id} not found.");

                    _logger.LogError(message.ToString());

                    return new InvokeResult(false, message.ToString());
                }

                // active operation
                operation.SetActive();
                _repositoryService.Update(operation);
                _repositoryService.SaveChanges();

                message.AppendLine($"Active Operation {id}. ExecutionName {operation.ExecutionName}.");

                var operationExecution = new OperationFactory(operation, _logger, _commandDispatcher).GetOperationBasedOnOperation();

                if (operationExecution == null)
                {
                    message.AppendLine($"Not found Operation {id}, ExecutionName {operation.ExecutionName}.");

                    return new InvokeResult(false, message.ToString());
                }

                // PreProcessing prepare command -> Processing execute command -> PostProcess find route to toOperation
                message.AppendLine($"Operation {id}. PreProcessing prepare command -> Processing execute command -> PostProcess find route to toOperation");

                operationExecution.PreProcessing().IsValid().Processing().Result.IsValid().PostProcessing().IsValid();

                var invokeResult = new InvokeResult(true, message.ToString());

                // add message operation after process
                invokeResult.AddMessage(operationExecution.GetMessage());

                return invokeResult;
            }
            catch (Exception ex)
            {
                message.AppendLine(ex.ToString());

                _logger.LogError(message.ToString());

                return new InvokeResult(false, message.ToString());
            }
        }

    }
}
