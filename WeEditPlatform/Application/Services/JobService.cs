using Application.Models;
using Application.Operations.Actions.AssignAction;
using Application.Queries;
using Domain;
using Infrastructure.Pagging;
using Infrastructure.Repository;
using Newtonsoft.Json;

namespace Application.Services
{
    public class JobService : IJobService
    {
        private readonly IRepositoryService _repositoryService;

        public JobService(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public Job CreateJob(CreateJobVM request)
        {
            try
            {
                var location = (Location)request.LocationId;
                var deliveryType = (DeliverType)request.DeliverTypeId;
                var app = (App)request.AppId;
                var productLevel = _repositoryService.Find<ProductLevel>(request.ProductLevelId);
                var CSOStaff = _repositoryService.Find<Staff>(request.CSOStaffId);

                if (productLevel == null)
                {
                    throw new Exception("ProductLevel dose not exist");
                }

                if (CSOStaff == null)
                {
                    throw new Exception("CSO Staff dose not exist");
                }

                var job = Job.Create(request.Date, location, CSOStaff, request.JobName, request.Code,
                    request.Instruction, request.InputInfo, request.InputNumber,
                    deliveryType, productLevel, request.Deadline, app);

                var jobCreated = _repositoryService.Add(job);

                var result = _repositoryService.SaveChanges();
                if (!result)
                {
                    throw new Exception("Error create new job");
                }

                return jobCreated;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public Job UpdateJobStatus(UpdateJobStatusVM request)
        {
            try
            {
                var job = _repositoryService.Find<Job>(request.JobId);

                if (job == null)
                {
                    throw new Exception("Job does not existing");
                }

                job.UpdateStatus(request.JobStatus);
                _repositoryService.Update(job);
                _repositoryService.SaveChanges();

                return job;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Job FindJob(int jobId, bool isInclude)
        {
            var job = _repositoryService.Find<Job>(jobId, new JobSpecification(isInclude));

            if (job == null)
            {
                throw new Exception("Job does not existing");
            }

            return job;
        }

        public List<Job> FindJobs(int[] jobIds, bool isInclude)
        {
            var jobs = _repositoryService.List<Job>(jobIds, new JobSpecification(isInclude));

            if (jobs == null || !jobs.Any())
            {
                throw new Exception("Jobs does not existing");
            }

            return jobs;
        }


        public List<Job> FindJobs(int pageNumber, int pageSize, string columnOrders, string searchValue, bool isInclude, out int totalRecords)
        {
            var jobSpecification = new JobSpecification(isInclude, searchValue, columnOrders.ToColumnOrders());
            var pagedjobs = _repositoryService.Find<Job>(pageNumber, pageSize, jobSpecification, out totalRecords).ToList();
            return pagedjobs;
        }

        public List<JobStep> AddStepsToJob(AddStepsToJobVM request)
        {
            try
            {
                var steps = _repositoryService.List<Step>(request.StepIds);
                var job = _repositoryService.Find<Job>(request.JobId, new JobSpecification(true));

                if (job == null)
                {
                    throw new Exception("Job does not existing");
                }

                if (steps == null || !steps.Any())
                {
                    throw new Exception($"Steps {request.StepIds} does not existing");
                }

                steps.ForEach(s =>
                {
                    job.AddStep(s);
                });

                _repositoryService.Update(job);
                _repositoryService.SaveChanges();

                return FindJobStepsOfJob(request.JobId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<JobStep> RemoveStepsFromJob(RemoveStepsFromJobVM request)
        {
            try
            {
                var job = _repositoryService.Find<Job>(request.JobId, new JobSpecification(true));

                if (job == null)
                {
                    throw new Exception("Job does not existing");
                }

                var steps = _repositoryService.List<Step>(request.StepIds).ToArray();

                if (steps == null || !steps.Any())
                {
                    throw new Exception($"Steps does not existing");
                }

                job.RemoveSteps(steps);
                _repositoryService.Update(job);
                _repositoryService.SaveChanges();

                return FindJobStepsOfJob(request.JobId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<JobStep> AssignStaffToStep(AssignStaffToStepVM request)
        {
            try
            {
                var job = _repositoryService.Find<Job>(request.JobId, new JobSpecification(true));

                if (job == null)
                {
                    throw new Exception("Job does not existing");
                }

                var staff = _repositoryService.Find<Staff>(request.StaffId);
                if (staff == null)
                {
                    throw new Exception("Staff does not existing");
                }

                var step = _repositoryService.Find<Step>(request.StepId);
                if (step == null)
                {
                    throw new Exception("Step does not existing");
                }

                job.AssignWorkerToStep(staff, step, request.EstimationInSeconds);
                staff.SetAssigned();

                _repositoryService.Update(job);
                _repositoryService.Update(staff);

                _repositoryService.SaveChanges();

                var jobSteps = _repositoryService.List<JobStep>(w => w.JobId == request.JobId).ToList();
                return jobSteps;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<JobStep> UnAssignStaffOfJobs(int staffId)
        {
            try
            {
                var staff = _repositoryService.Find<Staff>(s => s.Id == staffId);
                if (staff == null)
                {
                    throw new Exception("Staff not found");
                }

                var jobSteps = _repositoryService.List<JobStep>(s => (s.Status == StepStatus.Assigned || s.Status == StepStatus.Doing)
                && s.WorkerId == staffId).ToList();

                if (jobSteps.Any())
                {
                    jobSteps.ForEach(jobStep =>
                    {
                        jobStep.SetNoneWorkerShift();
                        jobStep.UpdateStatus(StepStatus.Todo);
                    });

                    _repositoryService.Update(jobSteps.ToArray());
                    _repositoryService.SaveChanges();
                }

                return jobSteps;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        const string matchingAssignSetting = "{\"IsGroupMatching\": true, \"IsProductLevelMatching\" : true}";
        const string validJobStepIsExpriedMethod = "jobstep.isExpried";

        public List<JobStep> AutoAssignStaffToStep()
        {
            try
            {
                var assignedJobSteps = new List<JobStep>();

                // Scan Jobs have Todo Steps
                var todoJobSteps = _repositoryService.List<JobStep>(w => w.Status == null || w.Status == StepStatus.Todo).ToList();

                // And Not Expired time
                if (!string.IsNullOrEmpty(validJobStepIsExpriedMethod))
                {
                    todoJobSteps = todoJobSteps.Where(s => !s.IsExpried()).ToList();
                }

                // Matching Staff and Step by ProductLevel
                if (!todoJobSteps.Any())
                {
                    return new List<JobStep>();
                }

                // Scan free Staffs and active
                var freeStaffs = _repositoryService.Find<Staff>(new StaffFreeSpecification()).ToList();
                if (freeStaffs == null && !freeStaffs.Any())
                {
                    return new List<JobStep>();
                }

                var freeActiveStaffs = freeStaffs.Where(s => s.IsActive()).ToList();
                if (freeActiveStaffs == null || !freeActiveStaffs.Any())
                {
                    return new List<JobStep>();
                }

                return AssignOperation(todoJobSteps, freeActiveStaffs, JsonConvert.DeserializeObject<MatchingAssignSetting>(matchingAssignSetting));
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private List<JobStep> AssignOperation(List<JobStep> todoJobSteps, List<Staff> freeActiveStaffs,
            MatchingAssignSetting matchingAssignSetting)
        {
            var assignedJobSteps = new List<JobStep>();
            var steps = _repositoryService.All<Step>();

            foreach (var jobStep in todoJobSteps)
            {
                var matchedStaffs = new List<Staff>();
                var step = steps.FirstOrDefault(s => s.Id == jobStep.StepId);

                // And Group matching
                if (matchingAssignSetting.IsGroupMatching)
                {
                    matchedStaffs = freeActiveStaffs.Where(s => s.Groups != null && s.Groups.Select(g => g.Id).Contains(step.GroupId)).ToList();
                }

                // And ProductLevel matching
                if (matchingAssignSetting.IsProductLevelMatching)
                {
                    matchedStaffs = matchedStaffs.Where(s => s.ProductLevels != null &&
                        s.ProductLevels.Contains(step.ProductLevel)).ToList();
                }

                if (matchedStaffs != null && matchedStaffs.Any())
                {
                    var matchedStaff = matchedStaffs.FirstOrDefault();

                    // Assign staff to step, set status Assigned
                    jobStep.SetWorkerAtShift(matchedStaff.Id, matchedStaff.GetCurrentShift().Id);
                    jobStep.UpdateStatus(StepStatus.Assigned);
                    jobStep.SetEstimationInSeconds(step.EstimationInSeconds);
                    matchedStaff.SetAssigned();

                    // save update assigned
                    _repositoryService.Update(jobStep);
                    _repositoryService.Update(matchedStaff);
                    _repositoryService.SaveChanges();

                    assignedJobSteps.Add(jobStep);
                }
            }

            return assignedJobSteps;
        }

        public List<JobStep> FindJobStepsOfJob(params int[] jobIds)
        {
            return _repositoryService.Find<JobStep>(new JobStepsOfJobSpecification(jobIds, true)).ToList();
            //return _repositoryService.List<JobStep>(w => jobIds.Contains(w.JobId)).ToList();
        }

        public void RemoveJob(int id)
        {
            var job = _repositoryService.Find<Job>(id);

            if (job == null)
            {
                throw new Exception("Job does not existing");
            }

            _repositoryService.Delete(job);
            _repositoryService.SaveChanges();
        }


    }
}
