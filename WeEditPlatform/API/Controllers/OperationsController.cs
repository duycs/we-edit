using Application.Models;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OperationsController : ControllerBase
    {
        private readonly ILogger<OperationsController> _logger;
        private readonly IOperationService _operationService;

        public OperationsController(
            IOperationService operationService,
            ILogger<OperationsController> logger)
        {
            _logger = logger;
            _operationService = operationService;
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddOperation(CreateOperationVM request)
        {
            var operationCreated = _operationService.CreateOperation(request);
            return Ok(operationCreated);
        }

        [Authorize]
        [HttpPut]
        public IActionResult UpdateOperation(UpdateOperationVM request)
        {
            var operationUpdated = _operationService.UpdateOperation(request);
            return Ok(operationUpdated);
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetOperation(int id, bool isInclude)
        {
            var operationCreated = _operationService.GetOperation(id, isInclude);
            return Ok(operationCreated);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteOperation(int id)
        {
            _operationService.RemoveOperation(id);
            return Ok();
        }
    }
}