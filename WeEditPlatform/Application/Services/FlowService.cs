using Application.Models;
using Application.Queries;
using Domain;
using Infrastructure.Models;
using Infrastructure.Pagging;
using Infrastructure.Repository;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Application.Services
{
    public class FlowService : IFlowService
    {
        private readonly ILogger<FlowService> _logger;
        private readonly IRepositoryService _repositoryService;
        private readonly IRouteService _routeService;
        private readonly IOperationService _operationService;

        public FlowService(ILogger<FlowService> logger,
            IRepositoryService repositoryService,
            IRouteService routeService,
            IOperationService operationService
            )
        {
            _logger = logger;
            _repositoryService = repositoryService;
            _routeService = routeService;
            _operationService = operationService;
        }

        public Flow CreateFlow(CreateFlowVM request)
        {
            var flow = _repositoryService.Add(Flow.Create(request.Name, request.Description, request.Status, request.Type));
            _repositoryService.SaveChanges();
            return flow;
        }

        public List<Flow> FindFlows(int pageNumber, int pageSize, string columnOrders, string searchValue, bool isInclude, out int totalRecords)
        {
            var flowSpecification = new FlowSpecification(isInclude, searchValue, columnOrders.ToColumnOrders());
            var pagedFlows = _repositoryService.Find<Flow>(pageNumber, pageSize, flowSpecification, out totalRecords).ToList();
            return pagedFlows;
        }

        public Flow FindFlow(int id, bool isInclude)
        {
            return _repositoryService.Find<Flow>(id, new FlowSpecification(isInclude));
        }

        public List<Flow> FindFlows(int[] ids, bool isInclude)
        {
            return _repositoryService.List<Flow>(ids, new FlowSpecification(isInclude));
        }

        public Flow UpdateFlow(UpdateFlowVM request)
        {
            var flow = _repositoryService.Find<Flow>(request.Id);

            if (flow == null)
            {
                throw new Exception($"Flow {request.Id} not found");
            }

            var flowUpdated = flow.Update(request.Name, request.Description, request.Status, request.Type);

            _repositoryService.Update(flowUpdated);
            _repositoryService.SaveChanges();

            return flowUpdated;
        }

        public void RemoveFlow(int id)
        {
            var flow = _repositoryService.Find<Flow>(id);

            if (flow == null)
            {
                throw new Exception($"Flow {id} not found");
            }

            _repositoryService.Delete(flow);
            _repositoryService.SaveChanges();
        }

        /// <summary>
        /// Run Flow from from First to End of Operations via Routes
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<InvokeResult> RunFlow(int id)
        {
            var invokeResult = new InvokeResult(false);
            try
            {
                invokeResult.AddMessage($"Invoke Flow {id}");

                var flow = _repositoryService.Find(id, new FlowSpecification(true));

                if (flow == null)
                {
                    invokeResult.AddMessage($"Flow not found");

                    _logger.LogError(invokeResult.GetMessage());

                    return invokeResult;
                }

                var firstRouteOperation = flow.FirstRouteOperation();
                if (firstRouteOperation == null)
                {
                    invokeResult.AddMessage($"Do not exist any Operations").SetSuccessFalse();

                    _logger.LogError(invokeResult.GetMessage());

                    return invokeResult;
                }

                // Fire first Operaion
                // eg: first Route: A -> B
                var fromOperationRoute = _routeService.FindRoutesOfFromOperation(firstRouteOperation.Id).FirstOrDefault();

                // eg: invoke A
                invokeResult = await _operationService.Invoke(firstRouteOperation.Id);

                invokeResult.AddMessage(@$"Invoke first Operation {firstRouteOperation.Id}").AddMessage($"Success: {invokeResult.IsSuccess()}");

                if (!invokeResult.IsSuccess())
                {
                    invokeResult.AddMessage("Invoke Operation not success").SetSuccessFalse();

                    _logger.LogError(invokeResult.GetMessage());

                    return invokeResult;
                }

                // Next Operation if current Operation is success and next Route existing
                while (invokeResult.IsSuccess() && fromOperationRoute != null)
                {
                    // invoke B
                    invokeResult = await _operationService.Invoke(fromOperationRoute.ToOperationId);

                    if (!invokeResult.IsSuccess())
                    {
                        invokeResult.AddMessage("Invoke Operation not success").SetSuccessFalse();

                        _logger.LogError(invokeResult.GetMessage());

                        return invokeResult;
                    }

                    invokeResult.AddMessage(@$"Invoke Operation {fromOperationRoute.ToOperationId}").AddMessage($"Success: {invokeResult.IsSuccess()}");

                    // eg: route B -> C
                    fromOperationRoute = _routeService.FindRoutesOfFromOperation(fromOperationRoute.ToOperationId).FirstOrDefault();

                    if (fromOperationRoute != null)
                    {
                        invokeResult.AddMessage(@$"Route: from Operation {fromOperationRoute.FromOperationId} to Operation {fromOperationRoute.ToOperationId}");
                    }
                }

                // End route then exit the Flow
                invokeResult.AddMessage($"End Route, exit Flow {id}");

                _logger.LogInformation(invokeResult.GetMessage());

                return invokeResult;
            }
            catch (Exception ex)
            {
                invokeResult.AddMessage(ex.Message.ToString()).SetSuccessFalse();

                _logger.LogError(invokeResult.GetMessage());

                return invokeResult;
            }
        }
    }
}
