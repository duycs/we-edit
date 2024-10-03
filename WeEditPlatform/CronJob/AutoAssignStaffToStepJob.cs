using Application.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using System.Diagnostics;

namespace CronJob
{
    [DisallowConcurrentExecution]
    public class AutoAssignStaffToStepJob : IJob
    {
        public static readonly JobKey AUTO_ASSIGN_STAFF_TO_STEP_KEY = new JobKey("AutoAssignStaffToStepJob");
        public static readonly int INTERVAL_SECONDS = 5;
        private readonly ILogger<AutoAssignStaffToStepJob> _logger;
        private readonly IHubContext<JobsHub> _hubContext;
        private static int _counter = 0;
        private static IJobService _jobService { get; set; }

        public AutoAssignStaffToStepJob(ILogger<AutoAssignStaffToStepJob> logger,
               IHubContext<JobsHub> hubContext,
            IJobService jobService
         )
        {
            _logger = logger;
            _hubContext = hubContext;
            _jobService = jobService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var watch = Stopwatch.StartNew();
                var count = _counter++;
                var beginMessage = $"AutoAssignStaffToStepJob Job BEGIN {count} {DateTime.UtcNow}";

                _logger.LogInformation(beginMessage);

                var jobSteps = _jobService.AutoAssignStaffToStep().Select(w => new { w.JobId, w.StepId, w.WorkerId, w.Status });

                _logger.LogInformation("Auto Assign JobSteps: " + JsonConvert.SerializeObject(jobSteps));


                watch.Stop();

                var responseMessage = $"Auto assign Job {count}, execution time in {watch.ElapsedMilliseconds}ms";
                if (jobSteps.Any())
                {
                    responseMessage += $". Assigned {jobSteps.Count()} staffs for steps";
                }

                var endMessage = $"AutoAssignStaffToStepJob Job END {count} {DateTime.UtcNow}. {responseMessage}";

                var response = new { Count = count, Message = responseMessage, Data = jobSteps };

                await _hubContext.Clients.All.SendAsync("AutoAssignStaffToStepJob", response);

                _logger.LogInformation(endMessage);
            }
            catch (Exception ex)
            {
                throw new JobExecutionException(ex);
            }
        }
    }
}
