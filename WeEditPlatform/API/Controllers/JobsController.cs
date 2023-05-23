using Application.Models;
using Application.Services;
using Domain;
using Infrastructure.Pagging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly ILogger<JobsController> _logger;
        private readonly IJobService _jobService;
        private readonly IUriService _uriService;

        private readonly IHubContext<APIHub> _hub;
        private readonly TimerManager _timer;

        public JobsController(ILogger<JobsController> logger,
            IHubContext<APIHub> hub, TimerManager timer,
            IJobService jobService, 
            IUriService uriService)
        {
            _logger = logger;
            _hub = hub;
            _timer = timer;
            _jobService = jobService;
            _uriService = uriService;
        }

        [HttpGet("{id}")]
        public IActionResult GetJob(int id, [FromQuery] bool isInclude = true)
        {
            var job = _jobService.FindJob(id, isInclude);
            return Ok(job);
        }

        [HttpGet]
        public async Task<IActionResult> GetJobs([FromQuery] int pageNumber = 0, [FromQuery] int pageSize = 0, [FromQuery] string? columnOrders = "",
            [FromQuery] int[]? ids = null, [FromQuery] string? searchValue = "", [FromQuery] bool isInclude = true)
        {
            if (ids != null && ids.Any())
            {
                var jobs = _jobService.FindJobs(ids, isInclude);
                return Ok(jobs);
            }
            else
            {
                var filter = new PaginationFilterOrder(pageNumber, pageSize, columnOrders);
                var jobPagedData = _jobService.FindJobs(filter.PageNumber, filter.PageSize, filter.ColumnOrders, searchValue, true, out int totalRecords);
                var jobPagedReponse = PaginationHelper.CreatePagedReponse<Job>(jobPagedData, filter, totalRecords, _uriService, Request.Path.Value);

                if (!_timer.IsTimerStarted)
                    _timer.PrepareTimer(() => _hub.Clients.All.SendAsync("TransferJobData", jobPagedReponse));

                return Ok(jobPagedReponse);
            }
        }

        [HttpGet("{id}/jobsteps")]
        public async Task<IActionResult> GetJobStepsOfJob(int id)
        {
            var jobSteps = _jobService.FindJobStepsOfJob(id);
            return Ok(jobSteps);
        }

        [HttpPost]
        public async Task<IActionResult> AddJob([FromBody] CreateJobVM request)
        {
            var job = _jobService.CreateJob(request);
            return Ok(job);
        }

        [HttpPost("steps")]
        public async Task<IActionResult> AddStepsToJob([FromBody] AddStepsToJobVM request)
        {
            var jobSteps = _jobService.AddStepsToJob(request);

            // auto assign trigger
            _jobService.AutoAssignStaffToStep();

            return Ok(jobSteps);
        }

        [HttpPost("auto-assign")]
        public async Task<IActionResult> AutoAssignStaffToStep()
        {
            var jobSteps = _jobService.AutoAssignStaffToStep();
            return Ok(jobSteps);
        }

        [HttpPut("assign")]
        public async Task<IActionResult> AssignStaffToStep([FromBody] AssignStaffToStepVM request)
        {
            var jobSteps = _jobService.AssignStaffToStep(request);
            return Ok(jobSteps);
        }

        [HttpPut("status")]
        public async Task<IActionResult> UpdateJobStatus([FromBody] UpdateJobStatusVM request)
        {
            var job = _jobService.UpdateJobStatus(request);
            return Ok(job);
        }

        [HttpPut("steps")]
        public async Task<IActionResult> RemoveStepsFromJob([FromBody] RemoveStepsFromJobVM request)
        {
            var jobSteps = _jobService.RemoveStepsFromJob(request);
            return Ok(jobSteps);
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveJob(int id)
        {
            _jobService.RemoveJob(id);
            return Ok();
        }
    }
}