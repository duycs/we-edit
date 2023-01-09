using Microsoft.AspNetCore.SignalR;

namespace CronJob
{
    public class JobsHub : Hub
    {
        public Task SendAutoAssignStaffToStepJobMessage(string message)
        {
            return Clients.All.SendAsync("AutoAssignStaffToStepJob", message);
        }

        //public Task SendNonConcurrentJobsMessage(string message)
        //{
        //    return Clients.All.SendAsync("NonConcurrentJobs", message);
        //}

    }
}
