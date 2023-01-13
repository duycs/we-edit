using Application.Models;
using Domain;

namespace Application.Services
{
    public interface IRouteService
    {
        Route CreateRoute(CreateRouteVM request);
        Route UpdateRoute(UpdateRouteVM request);
        void RemoveRoute(int id);

        /// <summary>
        /// Insert a operation to route then create 2 new routes and remove old route
        /// </summary>
        /// <param name="request">Route and Operation</param>
        /// <returns></returns>
        List<Route> InsertOperationToRoute(InsertOperationToRouteVM request);

        Route FindRoute(int id);

        /// <summary>
        /// eg: route A -> B, find route of A Operation
        /// </summary>
        /// <param name="fromOperationId"></param>
        /// <returns></returns>
        List<Route> FindRoutesOfFromOperation(int fromOperationId);
    }
}
