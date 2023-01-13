using Application.Models;
using Domain;
using Infrastructure.Models;

namespace Application.Services
{
    public interface IFlowService
    {
        Flow CreateFlow(CreateFlowVM request);
        Flow UpdateFlow(UpdateFlowVM request);
        Flow FindFlow(int id, bool isInclude);
        List<Flow> FindFlows(int[] ids, bool isInclude);
        List<Flow> FindFlows(int pageNumber, int pageSize, string columnOrders, string searchValue, bool isInclude, out int totalRecords);
        void RemoveFlow(int id);
        Task<InvokeResult> RunFlow(int id);

    }
}
