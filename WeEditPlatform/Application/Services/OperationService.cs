using Application.Commands;
using Application.Models;
using Application.Queries;
using Domain;
using Infrastructure.Commands;
using Infrastructure.Extensions;
using Infrastructure.Repository;

namespace Application.Services
{
    public class OperationService : IOperationService
    {
        private readonly IRepositoryService _repositoryService;
        private readonly ISettingService _settingService;
        private readonly ICommandDispatcher _commandDispatcher;

        public OperationService(IRepositoryService repositoryService, ISettingService settingService, ICommandDispatcher commandDispatcher)
        {
            _repositoryService = repositoryService;
            _settingService = settingService;
            _commandDispatcher = commandDispatcher;
        }

        public Operation CreateOperation(CreateOperationVM request)
        {
            var flow = _repositoryService.Find<Flow>(request.FlowId);
            if (flow == null)
            {
                throw new Exception($"Flow {request.FlowId} not found");
            }

            var operation = _repositoryService.Add(Operation.Create(flow, request.Type, request.Name, request.Description, request.ExecutionName, request.Settings));
            _repositoryService.SaveChanges();

            return operation;
        }

        public Operation GetOperation(int id, bool isInclude)
        {
            return _repositoryService.Find<Operation>(id, new OperationSpecification(isInclude));
        }

        public void Invoke(int id)
        {
            var operation = _repositoryService.Find(id, new OperationSpecification(true));

            if (operation == null)
            {
                throw new Exception($"Operation {id} not found");
            }

            switch (operation.ExecutionName)
            {
                case "AssignAction":

                    if (operation.Settings == null || !operation.Settings.Any())
                    {
                        throw new Exception("Setting not found");
                    }

                    var settings = operation.Settings.ToList();

                    var assignActionCommand = new AssignActionCommand(
                        settings.GetValueSettingByKey(ClassExtension.GetMemberName((AssignActionCommand c) => c.MatchingAssignSetting)),
                        settings.GetValueSettingByKey(ClassExtension.GetMemberName((AssignActionCommand c) => c.RawSqlFilterJobStep)),
                        settings.GetValueSettingByKey(ClassExtension.GetMemberName((AssignActionCommand c) => c.ValidJobStepIsExpriedMethod)),
                        settings.GetValueSettingByKey(ClassExtension.GetMemberName((AssignActionCommand c) => c.RawSqlFilterStaff))
                        );

                    _commandDispatcher.Send(assignActionCommand);

                    break;

                default:
                    break;
            }
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

            var operationUpdated = operation.Update(request.Type, request.Name, request.Description, request.ExecutionName, request.Settings);

            _repositoryService.Update(operationUpdated);
            _repositoryService.SaveChanges();

            return operationUpdated;
        }
    }
}
