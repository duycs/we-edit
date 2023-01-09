using Application.Models;
using Domain;

namespace Application.Services
{
    public interface IJobService
    {
        Job CreateJob(CreateJobVM request);
        Job UpdateJobStatus(UpdateJobStatusVM request);
        Job FindJob(int jobId, bool isInclude);
        List<Job> FindJobs(int[] jobIds, bool isInclude);
        List<Job> FindJobs(int pageNumber, int pageSize, string columnOrders, string searchValue, bool isInclude, out int totalRecords);


        List<JobStep> AddStepsToJob(AddStepsToJobVM request);
        List<JobStep> RemoveStepsFromJob(RemoveStepsFromJobVM request);
        void RemoveJob(int id);
        List<JobStep> FindJobStepsOfJob(params int[] jobIds);
        List<JobStep> AssignStaffToStep(AssignStaffToStepVM request);

        List<JobStep> UnAssignStaffOfJobs(int staffId);

        /// <summary>
        /// Scan job get steps need to assign staff
        /// </summary>
        List<JobStep> AutoAssignStaffToStep();
    }
}