using Application.Models;
using Domain;
using Infrastructure.Repository;

namespace Application.Services
{
    public class RouteService : IRouteService
    {
        private readonly IRepositoryService _repositoryService;

        public RouteService(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public Route CreateRoute(CreateRouteVM request)
        {
            var fromOperation = _repositoryService.Find<Operation>(request.FromOperationId);

            if (fromOperation == null)
            {
                throw new Exception($"Operation {request.FromOperationId} not found");
            }

            var toOperation = _repositoryService.Find<Operation>(request.ToOperationId);

            if (toOperation == null)
            {
                throw new Exception($"Operation {request.ToOperationId} not found");
            }

            var route = Route.Create(request.FromOperationId, request.ToOperationId);

            _repositoryService.Add(route);
            _repositoryService.SaveChanges();

            return route;
        }

        public Route FindRoute(int id)
        {
            return _repositoryService.Find<Route>(id);
        }

        public List<Route> FindRoutesOfFromOperation(int fromOperationId)
        {
            var routes = _repositoryService.List<Route>(w => w.FromOperationId == fromOperationId).ToList();
            return routes;
        }

        public List<Route> InsertOperationToRoute(InsertOperationToRouteVM request)
        {
            var insertOperation = _repositoryService.Find<Operation>(request.InsertOperationId);

            if (insertOperation == null)
            {
                throw new Exception($"Operation {request.InsertOperationId} not found");
            }

            var oldRoute = _repositoryService.Find<Route>(request.RouteId);

            if (oldRoute == null)
            {
                throw new Exception($"Route {request.RouteId} not found");
            }

            // create 2 route: fromOperation to insertOperation, insertOperation to toOperation
            var newRoute1 = Route.Create(oldRoute.FromOperationId, insertOperation.Id);
            var newRoute2 = Route.Create(insertOperation.Id, oldRoute.ToOperationId);

            _repositoryService.Add(newRoute1);
            _repositoryService.Add(newRoute2);

            // remove old route
            _repositoryService.Delete(oldRoute);

            _repositoryService.SaveChanges();

            return new List<Route>() { newRoute1, newRoute2 };
        }

        public void RemoveRoute(int id)
        {
            var route = _repositoryService.Find<Route>(id);
            if (route == null)
            {
                throw new Exception($"Route {id} not found");
            }

            _repositoryService.Delete(route);
            _repositoryService.SaveChanges();
        }


        public Route UpdateRoute(UpdateRouteVM request)
        {
            var route = _repositoryService.Find<Route>(request.RouteId);
            if (route == null)
            {
                throw new Exception($"Route {request.RouteId} not found");
            }

            var fromOperation = _repositoryService.Find<Operation>(request.FromOperationId);
            var toOperation = _repositoryService.Find<Operation>(request.ToOperationId);
            var routeUpdated = route.Update(fromOperation.Id, toOperation.Id, request.Description);

            _repositoryService.Update(routeUpdated);
            _repositoryService.SaveChanges();

            return routeUpdated;
        }
    }
}
