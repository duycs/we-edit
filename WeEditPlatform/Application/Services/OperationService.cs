using Application.Models;
using Application.Operations;
using Application.Queries;
using Domain;
using Infrastructure.Commands;
using Infrastructure.Exceptions;
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
                throw new OperationException($"Flow {request.FlowId} not found");
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
                throw new OperationException($"Operation {id} not found");
            }

            _repositoryService.Delete(operation);
            _repositoryService.SaveChanges();
        }

        public Operation UpdateOperation(UpdateOperationVM request)
        {
            var operation = _repositoryService.Find<Operation>(request.Id);

            if (operation == null)
            {
                throw new OperationException($"Operation {request.Id} not found");
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
                throw new OperationException($"Operation {id} not found");
            }

            operation.SetActive();
            _repositoryService.Update(operation);
            _repositoryService.SaveChanges();
        }

        public async Task<InvokeResult> Invoke(int id)
        {
            var invokeResult = new InvokeResult(false);
            try
            {
                var operation = _repositoryService.Find(id, new OperationSpecification(true));

                if (operation == null)
                {
                    invokeResult.AddMessage($"Operation {id} not found").SetSuccessFalse();

                    _logger.LogError(invokeResult.GetMessage());

                    return invokeResult;
                }

                invokeResult = new InvokeResult(true, operation);

                // active operation
                operation.SetActive();
                _repositoryService.Update(operation);
                _repositoryService.SaveChanges();

                invokeResult.AddMessage("Active Operation");

                var operationExecution = new OperationFactory(operation, invokeResult, _logger, _commandDispatcher).GetOperationBasedOnOperation();

                if (operationExecution == null)
                {
                    invokeResult.AddMessage($"Not found ExecutionName {operation.ExecutionName}").SetSuccessFalse();

                    _logger.LogInformation(invokeResult.GetMessage());

                    return invokeResult;
                }

                // PreProcessing prepare command -> Processing execute command -> PostProcess find route to toOperation
                invokeResult = operationExecution.PreProcessing().IsValid().Processing().Result.IsValid().PostProcessing().IsValid().GetInvokeResult();

                return invokeResult;
            }
            catch (Exception ex)
            {
                invokeResult.AddMessage(ex.ToString()).SetSuccessFalse();

                _logger.LogError(invokeResult.GetMessage());

                return invokeResult;
            }
        }

    }
}
