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
    public class FlowsController : ControllerBase
    {
        private readonly IFlowService _flowService;
        private readonly ILogger<FlowsController> _logger;
        private readonly IUriService _uriService;

        public FlowsController(ILogger<FlowsController> logger,
            IUriService uriService,
            IFlowService flowService)
        {
            _logger = logger;
            _uriService = uriService;
            _flowService = flowService;
        }

        [Authorize(Policy = "staffPolicy")]
        [HttpGet]
        public IActionResult GetFlows([FromQuery] int pageNumber = 0, [FromQuery] int pageSize = 0, [FromQuery] string? columnOrders = "",
         [FromQuery] string? searchValue = "", [FromQuery] int[]? ids = null, [FromQuery] bool isInclude = true)
        {
            if (ids != null && ids.Any())
            {
                var flows = _flowService.FindFlows(ids, isInclude);
                return Ok(flows);
            }
            else
            {
                var filter = new PaginationFilterOrder(pageNumber, pageSize, columnOrders);
                var paggedFlows = _flowService.FindFlows(filter.PageNumber, filter.PageSize, filter.ColumnOrders, searchValue, isInclude, out int totalRecords);
                var pagedReponse = PaginationHelper.CreatePagedReponse<Flow>(paggedFlows, new PaginationFilterOrder(pageNumber, pageSize),
                    totalRecords, _uriService, Request.Path.Value);

                return Ok(pagedReponse);
            }
        }

        [Authorize(Policy = "staffPolicy")]
        [HttpGet("{id}")]
        public IActionResult GetFlow(int id, [FromQuery] bool isInclude = true)
        {
            var flow = _flowService.FindFlow(id, isInclude);
            return Ok(flow);
        }

        [Authorize(Policy = "staffPolicy")]
        [HttpPost]
        public IActionResult AddFlow(CreateFlowVM request)
        {
            var flow = _flowService.CreateFlow(request);
            return Ok(flow);
        }

        [Authorize(Policy = "staffPolicy")]
        [HttpPut]
        public IActionResult UpdateFlow(UpdateFlowVM request)
        {
            var flow = _flowService.UpdateFlow(request);
            return Ok(flow);
        }

        [Authorize(Policy = "staffPolicy")]
        [HttpDelete("{id}")]
        public IActionResult RemoveFlow(int id)
        {
            _flowService.RemoveFlow(id);
            return Ok();
        }
    }
}