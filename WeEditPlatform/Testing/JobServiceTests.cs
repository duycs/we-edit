using Application.Models;
using Application.Services;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Testing
{
    public class JobServiceTests : TestBase
    {
        private IJobService _jobService;
        private IStaffService _staffService;

        [OneTimeSetUp]
        public void Init()
        {
            _jobService = _serviceProvider.GetService<IJobService>();
            _staffService = _serviceProvider.GetService<IStaffService>();

            Init_First_Staff_And_JobData();
        }

        [TearDown]
        public void Clean()
        {
            //_productionContext.Database.EnsureDeleted();
            //_productionContext.Dispose();
        }

        [Test]
        public void Create_New_Job_Success()
        {
            // arrange
            // add job 2
            var createJobVM = new CreateJobVM()
            {
                Date = DateTime.Now,
                LocationId = 2,
                CSOStaffId = 1,
                JobName = "Retouching",
                Instruction = "This is a high end property, can you work your magic on these please",
                InputInfo = "{imageNumber: 30}",
                InputNumber = 100,
                DeliverTypeId = 2,
                ProductLevelId = 3,
                Deadline = DateTime.Now.AddDays(1),
                AppId = 1
            };

            // act
            var jobCreated = _jobService.CreateJob(createJobVM);

            // assert
            Assert.IsNotNull(jobCreated);

            // job id 2
            Assert.NotZero(jobCreated.Id);
        }

        [Test]
        [TestCase(1, JobStatus.Doing)]
        public void Update_Job_To_Doing_Status_Success(int jobId, JobStatus jobStatus)
        {
            // act
            var updateJobStatusVM = new UpdateJobStatusVM()
            {
                JobId = jobId,
                JobStatus = jobStatus
            };
            _jobService.UpdateJobStatus(updateJobStatusVM);

            // assert
            var jobUpdated = _jobService.FindJob(jobId, true);
            Assert.NotNull(jobUpdated);
            Assert.AreEqual(JobStatus.Doing, jobUpdated.Status);
        }

        [Test]
        [TestCase(1, new int[] { 1, 2 })]
        public void Add_Steps_Then_Remove_Steps_Success(int jobId, int[] stepIds)
        {
            // arrange
            var addStepsToJobVM = new AddStepsToJobVM()
            {
                JobId = jobId,
                StepIds = stepIds
            };

            // act add steps
            _jobService.AddStepsToJob(addStepsToJobVM);

            // assert exist 2 steps and jobsteps
            var jobAdded = _jobService.FindJob(1, true);
            var jobSteps = _jobService.FindJobStepsOfJob(jobId);
            Assert.True(jobAdded.Steps.Any(s => s.Id == stepIds[0]));
            Assert.True(jobAdded.Steps.Any(s => s.Id == stepIds[1]));
            Assert.NotNull(jobSteps);
            Assert.True(jobSteps.Any(w => w.JobId == jobId && w.StepId == stepIds[0]));
            Assert.True(jobSteps.Any(w => w.JobId == jobId && w.StepId == stepIds[1]));

            // arrange
            var removeStepsFromJobVM = new RemoveStepsFromJobVM()
            {
                JobId = jobId,
                StepIds = stepIds
            };

            // act remove steps
            _jobService.RemoveStepsFromJob(removeStepsFromJobVM);

            // assert empty steps
            var jobRemovedSteps = _jobService.FindJob(1, true);
            Assert.False(jobRemovedSteps.Steps.Any());
        }


        [Test]
        [TestCase(1)]
        public void Can_Not_Add_Duplicate_Steps_To_Job_Success(int jobId)
        {
            // arrange
            var addStepsToJobVM = new AddStepsToJobVM()
            {
                JobId = jobId,
                StepIds = new int[] { 1 }
            };

            // act add steps
            var job1Steps = _jobService.AddStepsToJob(addStepsToJobVM);
            var job2Steps = _jobService.AddStepsToJob(addStepsToJobVM);

            // assert
            Assert.NotNull(job1Steps);
            Assert.NotNull(job2Steps);
            Assert.AreEqual(1, job1Steps.Count());
            Assert.AreEqual(1, job2Steps.Count());
        }


        /// <summary>
        /// Job have steps 1,2,3. Assign staff 1 to step 1 of job
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="staffId"></param>
        /// <param name="stepId"></param>
        /// <param name="addStepIds"></param>
        [Test]
        [TestCase(1, 1, 1, new int[] { 1, 2, 3 })]
        public void Assign_Staff_To_Step_Success(int jobId, int staffId, int stepId, int[] addStepIds)
        {
            // arrange
            // at setup already has first Job id 1 and first Staff id 1 

            // add steps 1,2,3
            var addStepsToJobVM = new AddStepsToJobVM()
            {
                JobId = jobId,
                StepIds = addStepIds
            };
            _jobService.AddStepsToJob(addStepsToJobVM);

            // assign steps
            var assignStaffToStepVM = new AssignStaffToStepVM()
            {
                JobId = jobId,
                StaffId = staffId,
                StepId = stepId
            };

            // act
            _jobService.AssignStaffToStep(assignStaffToStepVM);

            // assert
            var assignedJob = _jobService.FindJob(jobId, true);
            var assingedJobSteps = _jobService.FindJobStepsOfJob(jobId);
            Assert.IsNotNull(assignedJob);
            Assert.IsNotNull(assignedJob.JobSteps);
            Assert.True(assingedJobSteps.Any(w => w.JobId == jobId && w.StepId == addStepIds[0]));
            Assert.True(assingedJobSteps.Any(w => w.JobId == jobId && w.StepId == addStepIds[1]));
            Assert.True(assingedJobSteps.Any(w => w.JobId == jobId && w.StepId == addStepIds[2]));
        }

        private void Init_First_Staff_And_JobData()
        {
            // first cso staff id 1
            _repositoryService.Add(Staff.Create("1b709d24-a987-4a47-ba38-94881005fbac", "Fullname CSO Staff", "CSOStaffAccount", "csoemail@gmail.com"));
            _repositoryService.SaveChanges();

            // first job id 1
            var createJobVM = new CreateJobVM()
            {
                Date = DateTime.Now,
                LocationId = 1,
                CSOStaffId = 1,
                JobName = "Retouching",
                Instruction = "This is a high end property, can you work your magic on these please",
                InputInfo = "{imageNumber: 30}",
                InputNumber = 30,
                DeliverTypeId = 1,
                ProductLevelId = 1,
                Deadline = DateTime.Now.AddDays(1),
                AppId = 1
            };
            _jobService.CreateJob(createJobVM);

            var productLevels = _repositoryService.List<ProductLevel>(new int[] { 1, 2, 3, 4 });

            // steps
            _repositoryService.Add(new Step[]
            {
                Step.Create("All", "All", 1, productLevels[0]),
                Step.Create("AllQC", "AllQC", 2, productLevels[1]),
                Step.Create("Windown", "Windown", 3, productLevels[2]),
                Step.Create("WindownQC", "WindownQC", 4, productLevels[3]),
            });
            _repositoryService.SaveChanges();
        }
    }
}
