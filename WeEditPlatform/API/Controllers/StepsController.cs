using Application.Models;
using Application.Queries;
using Domain;
using Infrastructure.Pagging;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Policy = "apiPolicy")]
    [ApiController]
    [Route("[controller]")]
    public class StepsController : ControllerBase
    {

        private readonly ILogger<StepsController> _logger;
        private readonly IUriService _uriService;
        private IRepositoryService _repositoryService;

        public StepsController(ILogger<StepsController> logger,
            IUriService uriService,
            IRepositoryService repositoryService)
        {
            _logger = logger;
            _uriService = uriService;
            _repositoryService = repositoryService;
        }

        [Authorize(Policy = "staffPolicy")]
        [HttpGet]
        public IActionResult GetSteps([FromQuery] int pageNumber = 0, [FromQuery] int pageSize = 0, [FromQuery] string? columnOrders = "",
          [FromQuery] int[]? ids = null, [FromQuery] string? searchValue = "", [FromQuery] bool isInclude = true)
        {
            if (pageNumber == 0 && pageSize == 0)
            {
                var steps = _repositoryService.All<Step>();
                return Ok(steps);
            }
            if (ids != null && ids.Any())
            {
                var steps = _repositoryService.List<Step>(ids);
                return Ok(steps);
            }
            else
            {
                var filter = new PaginationFilterOrder(pageNumber, pageSize, columnOrders);
                var paggedSteps = _repositoryService.Find<Step>(filter.PageNumber, filter.PageSize, new StepSpecification(isInclude, searchValue, columnOrders.ToColumnOrders()), out int totalRecords).ToList();
                var stepPagedReponse = PaginationHelper.CreatePagedReponse<Step>(paggedSteps, filter, totalRecords, _uriService, Request.Path.Value);

                return Ok(stepPagedReponse);
            }
        }

        [Authorize(Policy = "staffPolicy")]
        [HttpGet("{id}")]
        public IActionResult GetStep(int id)
        {
            var step = _repositoryService.Find<Step>(s => s.Id == id);
            return Ok(step);
        }

        [Authorize(Policy = "staffPolicy")]
        [HttpPost]
        public IActionResult AddStep([FromBody] CreateStepVM request)
        {
            var productLevel = _repositoryService.Find<ProductLevel>(request.ProductLevelId);
            if (productLevel == null)
            {
                return BadRequest("ProductLevel not found");
            }

            var group = _repositoryService.Find<Group>(request.GroupId);
            if (group == null)
            {
                return BadRequest("Group not found");
            }

            var step = Step.Create(request.Name, request.Code, request.OrderNumber, productLevel, request.GroupId, request.EstimationInSeconds);

            var stepCreated = _repositoryService.Add<Step>(step);
            _repositoryService.SaveChanges();

            return Ok(stepCreated);
        }

        [Authorize(Policy = "staffPolicy")]
        [HttpPut]
        public IActionResult UpdateStep([FromBody] UpdateStepVM request)
        {
            var step = _repositoryService.Find<Step>(request.StepId);
            if (step == null)
            {
                return BadRequest("Step not found");
            }

            var productLevel = _repositoryService.Find<ProductLevel>(request.ProductLevelId);
            if (productLevel == null)
            {
                return BadRequest("ProductLevel not found");
            }

            var group = _repositoryService.Find<Group>(request.GroupId);
            if (group == null)
            {
                return BadRequest("Group not found");
            }

            var stepUpdated = step.UpdateStep(request.Name, productLevel, request.GroupId, request.EstimationInSeconds);

            _repositoryService.Update<Step>(stepUpdated);
            _repositoryService.SaveChanges();

            return Ok(stepUpdated);
        }

        [Authorize(Policy = "staffPolicy")]
        [HttpDelete("{id}")]
        public IActionResult Remove(int id)
        {
            var step = _repositoryService.Find<Step>(s => s.Id == id);
            if (step == null)
            {
                return BadRequest("Step not found");
            }

            _repositoryService.Delete(step);
            _repositoryService.SaveChanges();

            return Ok(step);
        }
    }
}