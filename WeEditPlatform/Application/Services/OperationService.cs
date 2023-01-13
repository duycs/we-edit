using Application.Commands;
using Application.Models;
using Application.Queries;
using Domain;
using Infrastructure.Commands;
using Infrastructure.Extensions;
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
            string message = "";
            try
            {
                var operation = _repositoryService.Find(id, new OperationSpecification(true));

                if (operation == null)
                {
                    message = $"Operation {id} not found.";
                    _logger.LogError(message);

                    return new InvokeResult(false, message);
                }

                // active operation
                operation.SetActive();
                _repositoryService.Update(operation);
                _repositoryService.SaveChanges();

                message = $"Active Operation {id}. ExecutionName {operation.ExecutionName}.";
                _logger.LogInformation(message);

                switch (operation.ExecutionName)
                {
                    case "AssignAction":

                        if (operation.Settings == null || !operation.Settings.Any())
                        {
                            message = "Not found any Settings.";
                            _logger.LogError(message);

                            return new InvokeResult(false, message);
                        }

                        var settings = operation.Settings.ToList();

                        var assignActionCommand = new AssignActionCommand(
                            settings.GetValueSettingByKey(ClassExtension.GetMemberName((AssignActionCommand c) => c.MatchingAssignSetting)),
                            settings.GetValueSettingByKey(ClassExtension.GetMemberName((AssignActionCommand c) => c.RawSqlFilterJobStep)),
                            settings.GetValueSettingByKey(ClassExtension.GetMemberName((AssignActionCommand c) => c.ValidJobStepIsExpriedMethod)),
                            settings.GetValueSettingByKey(ClassExtension.GetMemberName((AssignActionCommand c) => c.RawSqlFilterStaff))
                            );

                        await _commandDispatcher.SendGetResponse(assignActionCommand);

                        break;

                    default:
                        message = $"ExecutionName {operation.ExecutionName} not exist.";
                        _logger.LogError(message);

                        return new InvokeResult(false, message);

                }

                return new InvokeResult(true, message);
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                _logger.LogError(message);

                return new InvokeResult(false, message);
            }
        }

    }
}
