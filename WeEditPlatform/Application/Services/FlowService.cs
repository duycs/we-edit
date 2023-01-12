using Application.Models;
using Application.Queries;
using Domain;
using Infrastructure.Pagging;
using Infrastructure.Repository;

namespace Application.Services
{
    public class FlowService : IFlowService
    {
        private readonly IRepositoryService _repositoryService;

        public FlowService(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
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
    }
}
