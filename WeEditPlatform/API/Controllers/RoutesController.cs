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

        [Authorize(Policy = "staffPolicy")]
        [HttpGet]
        public IActionResult GetRoutes([FromQuery] int fromOperationId)
        {
            var routes = _routeService.FindRoutesOfFromOperation(fromOperationId);
            return Ok(routes);
        }

        [Authorize(Policy = "staffPolicy")]
        [HttpGet("{id}")]
        public IActionResult GetRoute(int id)
        {
            var route = _routeService.FindRoute(id);
            return Ok(route);
        }

        [Authorize(Policy = "staffPolicy")]
        [HttpPost]
        public IActionResult AddRoute(CreateRouteVM request)
        {
            var route = _routeService.CreateRoute(request);
            return Ok(route);
        }

        [Authorize(Policy = "staffPolicy")]
        [HttpPut]
        public IActionResult UpdateRoute(UpdateRouteVM request)
        {
            var route = _routeService.UpdateRoute(request);
            return Ok(route);
        }

        [Authorize(Policy = "staffPolicy")]
        [HttpDelete("{id}")]
        public IActionResult RemoveRoute(int id)
        {
            _routeService.RemoveRoute(id);
            return Ok();
        }
    }
}