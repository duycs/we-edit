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
    public class ProductLevelsController : ControllerBase
    {
        private readonly ILogger<ProductLevelsController> _logger;
        private readonly IUriService _uriService;
        private IRepositoryService _repositoryService;

        public ProductLevelsController(ILogger<ProductLevelsController> logger,
            IUriService uriService,
            IRepositoryService repositoryService)
        {
            _logger = logger;
            _uriService = uriService;
            _repositoryService = repositoryService;
        }

        [Authorize(Policy = "staffPolicy")]
        [HttpGet("{id}")]
        public IActionResult GetProductLevel(int id)
        {
            var productLevel = _repositoryService.Find<ProductLevel>(id);
            return Ok(productLevel);
        }

        [Authorize(Policy = "staffPolicy")]
        [HttpGet]
        public IActionResult GetProductLevels([FromQuery] int pageNumber = 0, [FromQuery] int pageSize = 0, [FromQuery] string? columnOrders = "",
            [FromQuery] int[]? ids = null, [FromQuery] string? searchValue = "", [FromQuery] bool isInclude = true)
        {
            if (pageNumber == 0 && pageSize == 0)
            {
                var productLevels = _repositoryService.All<ProductLevel>();
                return Ok(productLevels);
            }
            else if (ids != null && ids.Any())
            {
                var productLevels = _repositoryService.List<ProductLevel>(ids);
                return Ok(productLevels);
            }
            else
            {
                var filter = new PaginationFilterOrder(pageNumber, pageSize, columnOrders);
                var productLevelSpecification = new ProductLevelSpecification(isInclude, searchValue, columnOrders.ToColumnOrders());
                var paggedProductLevels = _repositoryService.Find<ProductLevel>(filter.PageNumber, filter.PageSize, productLevelSpecification, out int totalRecords).ToList();
                var productLevelPagedReponse = PaginationHelper.CreatePagedReponse<ProductLevel>(paggedProductLevels, filter, totalRecords, _uriService, Request.Path.Value);

                return Ok(productLevelPagedReponse);
            }
        }

        [Authorize(Policy = "staffPolicy")]
        [HttpPost]
        public IActionResult AddProductLevel([FromBody] CreateProductLevelVM request)
        {
            var productLevel = _repositoryService.Add(ProductLevel.Create(request.Code, request.Name, request.Description));
            _repositoryService.SaveChanges();

            return Ok(productLevel);
        }

        [Authorize(Policy = "staffPolicy")]
        [HttpPut]
        public IActionResult UpdateProductLevel([FromBody] UpdateProductLevelVM request)
        {
            var productLevel = _repositoryService.Find<ProductLevel>(request.Id);

            if (productLevel == null)
            {
                return NotFound();
            }

            productLevel.Update(request.Code, request.Name, request.Description);
            _repositoryService.Update(productLevel);
            _repositoryService.SaveChanges();

            return Ok(productLevel);
        }

        [Authorize(Policy = "staffPolicy")]
        [HttpDelete("{id}")]
        public IActionResult DeleteProductLevel(int id)
        {
            var productLevel = _repositoryService.Find<ProductLevel>(id);

            if (productLevel == null)
            {
                return NotFound();
            }

            _repositoryService.Delete(productLevel);
            _repositoryService.SaveChanges();

            return Ok(productLevel);
        }
    }
}