using Application.Models;
using Application.Services;
using Domain;
using Infrastructure.Extensions;
using Infrastructure.Pagging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Policy = "apiPolicy")]
    [ApiController]
    [Route("[controller]")]
    public class StaffsController : ControllerBase
    {
        private readonly ILogger<StaffsController> _logger;
        private readonly IStaffService _staffService;
        private readonly INoteService _noteService;
        private readonly IJobService _jobService;
        private readonly ISSOService _ssoService;
        private readonly IUriService _uriService;

        public StaffsController(ILogger<StaffsController> logger,
            IJobService jobService,
            ISSOService ssoService,
            INoteService noteService,
            IStaffService staffService,
            IUriService uriService)
        {
            _logger = logger;
            _staffService = staffService;
            _jobService = jobService;
            _ssoService = ssoService;
            _noteService = noteService;
            _uriService = uriService;
        }

        //[Authorize]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult AddStaff([FromBody] CreateStaffVM request)
        {
            var staff = _staffService.CreateStaff(request);

            // auto assign trigger
            var assignedJobSteps = _jobService.AutoAssignStaffToStep();

            string message = $"Assigned StepIds: {string.Join(",", assignedJobSteps.Select(s => s.StepId).ToList())}";

            _logger.LogInformation($"staff created id: {staff.Id}", message);

            return Ok(staff);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateStaff([FromBody] UpdateStaffVM request)
        {
            var staff = _staffService.UpdateStaff(request);

            // auto assign trigger
            var assignedJobSteps = _jobService.AutoAssignStaffToStep();

            string message = $"Assigned StepIds: {string.Join(",", assignedJobSteps.Select(s => s.StepId).ToList())}";

            var roles = (staff.Roles == null || !staff.Roles.Any()) ? new string[] { } : staff.Roles.Select(r => r.Name).ToArray();
            await _ssoService.UpdateRoles(new AddRolesVM() { UserName = staff.Account, Roles = roles });

            return Ok(staff);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult RemoveStaff(int id)
        {
            _staffService.RemoveStaff(id);

            var unAssignJobSteps = _jobService.UnAssignStaffOfJobs(id);

            string message = $"Unassign StepIds: {string.Join(",", unAssignJobSteps.Select(s => s.StepId).ToList())}";

            return Ok(message);
        }

        [Authorize(Policy = "staffPolicy")]
        //[Authorize]
        [HttpGet("{id}")]
        public IActionResult GetStaff(string id, [FromQuery] bool isInclude = true)
        {
            if (id.IsUuid())
            {
                var staffByUserId = _staffService.FindStaffByUserId(id, isInclude);
                return Ok(staffByUserId);
            }

            var staffById = _staffService.FindStaff(int.Parse(id), isInclude);
            return Ok(staffById);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetStaffs([FromQuery] int pageNumber = 0, [FromQuery] int pageSize = 0, [FromQuery] string? columnOrders = "",
          [FromQuery] int[]? ids = null, [FromQuery] string? searchValue = "", [FromQuery] bool isInclude = true)
        {
            if (pageNumber == 0 && pageSize == 0)
            {
                var staffs = _staffService.FindAllStaffs();
                return Ok(staffs);
            }
            else if (ids != null && ids.Any())
            {
                var staffs = _staffService.FindStaffs(ids, isInclude);
                return Ok(staffs);
            }
            else
            {
                var filter = new PaginationFilterOrder(pageNumber, pageSize, columnOrders);
                var staffPagedData = _staffService.FindStaffs(filter.PageNumber, filter.PageSize, filter.ColumnOrders, searchValue, true, out int totalRecords);
                var staffPagedReponse = PaginationHelper.CreatePagedReponse<Staff>(staffPagedData, filter, totalRecords, _uriService, Request.Path.Value);
                return Ok(staffPagedReponse);
            }
        }

        [Authorize]
        [HttpGet("{id}/jobSteps")]
        public IActionResult GetJobStepsOfWorker(int id)
        {
            var jobStepDtos = new List<JobStepDto>();
            var jobSteps = _staffService.FindJobStepsOfWorker(id);

            if (jobSteps != null && jobSteps.Any())
            {
                foreach (var jobStep in jobSteps)
                {
                    var jobStepDto = new JobStepDto();
                    jobStepDto.JobStep = jobStep;
                    jobStepDto.Notes = _noteService.FindNotesByObject("jobStep", jobStep.Id);

                    jobStepDtos.Add(jobStepDto);
                }
            }

            return Ok(jobStepDtos);
        }

        [Authorize]
        [HttpPost("roles")]
        public IActionResult AddRolesForStaff([FromBody] AddRolesForStaffVM request)
        {
            _staffService.AddRolesForStaff(request);
            return Ok();
        }

        [Authorize]
        [HttpDelete("roles")]
        public IActionResult RemoveRolesForStaff([FromBody] RemoveRolesForStaffVM request)
        {
            _staffService.RemoveRolesForStaff(request);

            return Ok();
        }

        [Authorize]
        [HttpPost("productLevels")]
        public IActionResult AddProductLevelForStaff([FromBody] AddProductLevelsForStaffVM request)
        {
            _staffService.AddProductLevelForStaff(request);

            // auto assign trigger
            var assignedJobSteps = _jobService.AutoAssignStaffToStep();

            string message = $"Assigned StepIds: {string.Join(",", assignedJobSteps.Select(s => s.StepId).ToList())}";

            return Ok(message);
        }

        [Authorize]
        [HttpDelete("productLevels")]
        public IActionResult RemoveProductLevelForStaff([FromBody] RemoveProductLevelsForStaffVM request)
        {
            _staffService.RemoveProductLevelForStaff(request);

            // AutoUnAssign

            return Ok();
        }

        [Authorize]
        [HttpPost("shifts/out")]
        public IActionResult SetStaffOutShift([FromBody] StaffOutShiftVM request)
        {

            _staffService.SetStaffOutShift(request);

            // all jobStep of worker set Todo status and none worker, none shift
            var unAssignJobSteps = _jobService.UnAssignStaffOfJobs(request.StaffId);

            string message = $"Unassign StepIds: {string.Join(",", unAssignJobSteps.Select(s => s.StepId).ToList())}";

            return Ok(message);
        }

        [Authorize]
        [HttpPost("shifts/in")]
        public IActionResult SetStaffInShift([FromBody] StaffInShiftVM request)
        {
            _staffService.SetStaffInShift(request);

            // auto assign trigger
            var assignedJobSteps = _jobService.AutoAssignStaffToStep();

            string message = $"Assigned StepIds: {string.Join(",", assignedJobSteps.Select(s => s.StepId).ToList())}";

            return Ok(message);
        }

        [Authorize]
        [HttpPost("stepStatus")]
        public IActionResult UpdateStepStatus([FromBody] UpdateStepStatusVM request)
        {
            var jobStepUpdated = _staffService.UpdateStepStatus(request);
            return Ok(jobStepUpdated);
        }
    }
}