using Application.Models;
using Application.Services;
using Domain;
using Infrastructure.Pagging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoutesController : ControllerBase
    {
        private readonly IRouteService _routeService;
        private readonly ILogger<RoutesController> _logger;
        private readonly IUriService _uriService;

        public RoutesController(ILogger<RoutesController> logger,
            IUriService uriService,
            IRouteService routeService)
        {
            _logger = logger;
            _uriService = uriService;
            _routeService = routeService;
        }

       
        [HttpGet]
        public IActionResult GetRoutes([FromQuery] int fromOperationId)
        {
            var routes = _routeService.FindRoutesOfFromOperation(fromOperationId);
            return Ok(routes);
        }

       
        [HttpGet("{id}")]
        public IActionResult GetRoute(int id)
        {
            var route = _routeService.FindRoute(id);
            return Ok(route);
        }

       
        [HttpPost]
        public IActionResult AddRoute(CreateRouteVM request)
        {
            var route = _routeService.CreateRoute(request);
            return Ok(route);
        }

       
        [HttpPut]
        public IActionResult UpdateRoute(UpdateRouteVM request)
        {
            var route = _routeService.UpdateRoute(request);
            return Ok(route);
        }

       
        [HttpDelete("{id}")]
        public IActionResult RemoveRoute(int id)
        {
            _routeService.RemoveRoute(id);
            return Ok();
        }
    }
}