using Application.Models;
using Application.Services;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Linq;

namespace Testing
{
    public class AutoAssignJobTests : TestBase
    {
        private IJobService _jobService;
        private IStaffService _staffService;

        [SetUp]
        public void Init()
        {
            _jobService = _serviceProvider.GetService<IJobService>();
            _staffService = _serviceProvider.GetService<IStaffService>();

            Init_First_Staff_And_JobData();
        }

        [TearDown]
        public void Clean()
        {

        }

        [Test]
        [TestCase(1, 1, new int[] { 1, 2, 3 }, new int[] { 1, 2, 3 })]
        public void Auto_Assign_1_Worker_For_JobSteps_Of_Job_Success(int jobId, int staffId, int[] productLevelIds, int[] addStepIds)
        {
            // arrange
            // at setup already has first Job id 1 has productLevel1
            // already has first Staff id 1
            // add Staff has ProductLevel 1 
            _staffService.AddProductLevelForStaff(new AddProductLevelsForStaffVM() { ProductLevelIds = productLevelIds, StaffId = staffId });

            // Job 1 add steps 1,2,3
            var addStepsToJobVM = new AddStepsToJobVM()
            {
                JobId = jobId,
                StepIds = addStepIds
            };
            _jobService.AddStepsToJob(addStepsToJobVM);

            // staff in shift3 to have active status
            var now = DateTime.Now;
            var shift3At11h = new DateTime(now.Year, now.Month, now.Day, 23, 00, 00);
            _staffService.SetStaffInShift(new StaffInShiftVM() { StaffId = staffId, InShiftAt = shift3At11h });

            // act
            _jobService.AutoAssignStaffToStep();

            // assert
            var assignedJob = _jobService.FindJob(jobId, true);
            var assingedJobSteps = _jobService.FindJobStepsOfJob(jobId);
            Assert.IsNotNull(assignedJob);
            Assert.IsNotNull(assignedJob.Steps);

            // only one jobStep is assigned for staff id 1
            Assert.True(assingedJobSteps.Any(w => w.JobId == jobId && w.StepId == addStepIds[0] && w.WorkerId == 1));
            Assert.True(assingedJobSteps.Any(w => w.JobId == jobId && w.StepId == addStepIds[1] && w.WorkerId == 0));
            Assert.True(assingedJobSteps.Any(w => w.JobId == jobId && w.StepId == addStepIds[2] && w.WorkerId == 0));
        }

        [Test]
        [TestCase(1, new int[] { 1, 2, 3 }, new int[] { 1, 2, 3 }, new int[] { 1, 2, 3 })]
        public void Auto_Assign_3_Worker_For_3_JobSteps_With_3_ProductLevel_Success(int jobId, int[] staffIds, int[] staffProductLevelIds, int[] addStepIds)
        {
            // arrange
            // at setup already has first Job id 1 has productLevel1

            // Staff 1 has ProductLevel1, Staff 2 has ProductLevel2, Staff3 has ProductLevel3
            _staffService.AddProductLevelForStaff(new AddProductLevelsForStaffVM() { ProductLevelIds = staffProductLevelIds, StaffId = staffIds[0] });
            _staffService.AddProductLevelForStaff(new AddProductLevelsForStaffVM() { ProductLevelIds = staffProductLevelIds, StaffId = staffIds[1] });
            _staffService.AddProductLevelForStaff(new AddProductLevelsForStaffVM() { ProductLevelIds = staffProductLevelIds, StaffId = staffIds[2] });

            // Job 1 add steps 1,2,3 with ProductLevel 1,2,3
            var addStepsToJobVM = new AddStepsToJobVM()
            {
                JobId = jobId,
                StepIds = addStepIds
            };
            _jobService.AddStepsToJob(addStepsToJobVM);

            // staffs in shift3 to have active status
            var now = DateTime.Now;
            var shift3At11h = new DateTime(now.Year, now.Month, now.Day, 23, 00, 00);
            _staffService.SetStaffInShift(new StaffInShiftVM() { StaffId = staffIds[0], InShiftAt = shift3At11h });
            _staffService.SetStaffInShift(new StaffInShiftVM() { StaffId = staffIds[1], InShiftAt = shift3At11h });
            _staffService.SetStaffInShift(new StaffInShiftVM() { StaffId = staffIds[2], InShiftAt = shift3At11h });

            // act
            // auto assign staff active in Shift for Step todo same ProductLevel
            _jobService.AutoAssignStaffToStep();

            // assert
            // 3 JobSteps have been assigned 
            var assignedJob = _jobService.FindJob(jobId, true);
            var assingedJobSteps = _jobService.FindJobStepsOfJob(jobId);
            var staffs = _staffService.FindStaffs(staffIds, true);
            Assert.IsNotNull(assignedJob);
            Assert.IsNotNull(assignedJob.Steps);
            Assert.True(assignedJob.Steps.Count == 3);

            // 3 jobSteps is assigned for 3 staffs
            Assert.True(assingedJobSteps.Any(w => w.JobId == jobId && w.StepId == addStepIds[0] && w.WorkerId == 1));
            Assert.True(assingedJobSteps.Any(w => w.JobId == jobId && w.StepId == addStepIds[1] && w.WorkerId == 2));
            Assert.True(assingedJobSteps.Any(w => w.JobId == jobId && w.StepId == addStepIds[2] && w.WorkerId == 3));

            Assert.IsNotNull(staffs);
            Assert.True(staffs.Any(s => s.Id == staffIds[0] && s.IsAssigned));
            Assert.True(staffs.Any(s => s.Id == staffIds[1] && s.IsAssigned));
            Assert.True(staffs.Any(s => s.Id == staffIds[2] && s.IsAssigned));
        }

        [Test]
        [TestCase(1, new int[] { 1, 2, 3 }, new int[] { 1, 2, 3 }, new int[] { 1, 1, 1 })]
        public void Auto_Assign_3_Worker_Same_ProductLevel_For_1_JobSteps_Success(int jobId, int[] jobStepIds, int[] staffIds, int[] staffProductLevelIds)
        {
            // arrange
            // at setup already has first Job id 1 has productLevel1

            // 3 Staff has same ProductLevel 1
            _staffService.AddProductLevelForStaff(new AddProductLevelsForStaffVM() { ProductLevelIds = staffProductLevelIds, StaffId = staffIds[0] });
            _staffService.AddProductLevelForStaff(new AddProductLevelsForStaffVM() { ProductLevelIds = staffProductLevelIds, StaffId = staffIds[1] });
            _staffService.AddProductLevelForStaff(new AddProductLevelsForStaffVM() { ProductLevelIds = staffProductLevelIds, StaffId = staffIds[2] });

            // Job 1 add steps 1,2,3 with ProductLevel 1,2,3
            var addStepsToJobVM = new AddStepsToJobVM()
            {
                JobId = jobId,
                StepIds = jobStepIds
            };
            _jobService.AddStepsToJob(addStepsToJobVM);

            // staffs in shift3 to have active status
            var now = DateTime.Now;
            var shift3At11h = new DateTime(now.Year, now.Month, now.Day, 23, 00, 00);
            _staffService.SetStaffInShift(new StaffInShiftVM() { StaffId = staffIds[0], InShiftAt = shift3At11h });
            _staffService.SetStaffInShift(new StaffInShiftVM() { StaffId = staffIds[1], InShiftAt = shift3At11h });
            _staffService.SetStaffInShift(new StaffInShiftVM() { StaffId = staffIds[2], InShiftAt = shift3At11h });

            // act
            // auto assign staff active in Shift for Steps todo same ProductLevel
            _jobService.AutoAssignStaffToStep();

            // assert
            // 3 JobSteps have been assigned 
            var assignedJob = _jobService.FindJob(jobId, true);
            var assingedJobSteps = _jobService.FindJobStepsOfJob(jobId);
            var staffs = _staffService.FindStaffs(staffIds, true);
            Assert.IsNotNull(assignedJob);
            Assert.IsNotNull(assignedJob.Steps);
            Assert.True(assignedJob.Steps.Count == 3);

            // 3 jobSteps is assigned for 3 staffs
            Assert.True(assingedJobSteps.Any(w => w.JobId == jobId && w.StepId == jobStepIds[0] && w.WorkerId == 1));
            Assert.True(assingedJobSteps.Any(w => w.JobId == jobId && w.StepId == jobStepIds[1] && w.WorkerId == 0));
            Assert.True(assingedJobSteps.Any(w => w.JobId == jobId && w.StepId == jobStepIds[2] && w.WorkerId == 0));

            Assert.IsNotNull(staffs);
            Assert.True(staffs.Any(s => s.Id == staffIds[0] && s.IsAssigned));
            Assert.True(staffs.Any(s => s.Id == staffIds[1] && s.IsAssigned == false));
            Assert.True(staffs.Any(s => s.Id == staffIds[2] && s.IsAssigned == false));
        }

        [Test]
        [TestCase(1, new int[] { 1, 2, 3 }, new int[] { 1, 2, 3 }, new int[] { 2, 1, 1 })]
        public void Auto_Assign_3_Worker_For_2_JobSteps_Success(int jobId, int[] jobStepIds, int[] staffIds, int[] staffProductLevelIds)
        {
            // arrange
            // at setup already has first Job id 1 has productLevel1 and staff 1, 2, 3

            // set ProductLevels for staffs
            _staffService.AddProductLevelForStaff(new AddProductLevelsForStaffVM() { ProductLevelIds = staffProductLevelIds, StaffId = staffIds[0] });
            _staffService.AddProductLevelForStaff(new AddProductLevelsForStaffVM() { ProductLevelIds = staffProductLevelIds, StaffId = staffIds[1] });
            _staffService.AddProductLevelForStaff(new AddProductLevelsForStaffVM() { ProductLevelIds = staffProductLevelIds, StaffId = staffIds[2] });

            // Job 1 add steps 1,2,3 with ProductLevel 1,2,3
            var addStepsToJobVM = new AddStepsToJobVM()
            {
                JobId = jobId,
                StepIds = jobStepIds
            };
            _jobService.AddStepsToJob(addStepsToJobVM);

            // staffs in shift3 to have active status
            var now = DateTime.Now;
            var shift3At11h = new DateTime(now.Year, now.Month, now.Day, 23, 00, 00);
            _staffService.SetStaffInShift(new StaffInShiftVM() { StaffId = staffIds[0], InShiftAt = shift3At11h });
            _staffService.SetStaffInShift(new StaffInShiftVM() { StaffId = staffIds[1], InShiftAt = shift3At11h });
            _staffService.SetStaffInShift(new StaffInShiftVM() { StaffId = staffIds[2], InShiftAt = shift3At11h });

            // act
            // auto assign staff active in Shift for Steps todo same ProductLevel
            _jobService.AutoAssignStaffToStep();

            // assert
            // 3 JobSteps have been assigned 
            var assignedJob = _jobService.FindJob(jobId, true);
            var assingedJobSteps = _jobService.FindJobStepsOfJob(jobId);
            var staffs = _staffService.FindStaffs(staffIds, true);
            Assert.IsNotNull(assignedJob);
            Assert.IsNotNull(assignedJob.Steps);
            Assert.True(assignedJob.Steps.Count == 3);

            // Step productLevel 1 has assigned for staff 2 or 3
            // Step productLevel 2 has assigned for staff 1
            // Step ProductLevel 3 has not assigned
            Assert.True(assingedJobSteps.Any(w => w.JobId == jobId && w.StepId == jobStepIds[0] && w.WorkerId > 0));
            Assert.True(assingedJobSteps.Any(w => w.JobId == jobId && w.StepId == jobStepIds[1] && w.WorkerId == 1));
            Assert.True(assingedJobSteps.Any(w => w.JobId == jobId && w.StepId == jobStepIds[2] && w.WorkerId == 0));


            // Staff 1 is assigned, staff 3 not assigned
            Assert.IsNotNull(staffs);
            Assert.True(staffs.Any(s => s.Id == staffIds[0] && s.IsAssigned == true));
            Assert.True(staffs.Any(s => s.Id == staffIds[2] && s.IsAssigned == false));
        }

        [Test]
        [TestCase(1, new int[] { 1, 1, 3 }, new int[] { 1, 2, 3 }, new int[] { 1, 1, 2 })]
        public void Auto_Assign_2_Worker_For_2_JobSteps_Same_ProductLevel_Success(int jobId, int[] jobStepIds, int[] staffIds, int[] staffProductLevelIds)
        {
            // arrange
            // at setup already has first Job id 1 has productLevel1 and staff 1, 2, 3

            // set ProductLevel for staffs
            _staffService.AddProductLevelForStaff(new AddProductLevelsForStaffVM() { ProductLevelIds = staffProductLevelIds, StaffId = staffIds[0] });
            _staffService.AddProductLevelForStaff(new AddProductLevelsForStaffVM() { ProductLevelIds = staffProductLevelIds, StaffId = staffIds[1] });
            _staffService.AddProductLevelForStaff(new AddProductLevelsForStaffVM() { ProductLevelIds = staffProductLevelIds, StaffId = staffIds[2] });

            // Job 1 add steps 1,2,3 with ProductLevel 1,2,3
            var addStepsToJobVM = new AddStepsToJobVM()
            {
                JobId = jobId,
                StepIds = jobStepIds
            };
            var jobSteps1 = _jobService.AddStepsToJob(addStepsToJobVM);

            // staffs in shift3 to have active status
            var now = DateTime.Now;
            var shift3At11h = new DateTime(now.Year, now.Month, now.Day, 23, 00, 00);
            _staffService.SetStaffInShift(new StaffInShiftVM() { StaffId = staffIds[0], InShiftAt = shift3At11h });
            _staffService.SetStaffInShift(new StaffInShiftVM() { StaffId = staffIds[1], InShiftAt = shift3At11h });
            _staffService.SetStaffInShift(new StaffInShiftVM() { StaffId = staffIds[2], InShiftAt = shift3At11h });

            // act
            // auto assign staff active in Shift for Steps todo same ProductLevel
            var assignedJobSteps = _jobService.AutoAssignStaffToStep();

            // assert
            var job = _jobService.FindJob(jobId, true);
            var jobSteps = _jobService.FindJobStepsOfJob(jobId);
            var staffs = _staffService.FindStaffs(staffIds, true);
            Assert.IsNotNull(job);
            Assert.IsNotNull(assignedJobSteps);
            Assert.IsNotNull(job.Steps);
            Assert.True(job.Steps.Count == 2);
            Assert.True(assignedJobSteps.Count == 1);

            Assert.True(jobSteps.Any(w => w.JobId == jobId && w.StepId == jobStepIds[0] && w.WorkerId > 0));
            Assert.True(jobSteps.Any(w => w.JobId == jobId && w.StepId == jobStepIds[1] && w.WorkerId > 0));
            Assert.True(jobSteps.Any(w => w.JobId == jobId && w.StepId == jobStepIds[2] && w.WorkerId == 0));

            Assert.IsNotNull(staffs);
            Assert.True(staffs.Any(s => s.Id == staffIds[0] && s.IsAssigned == true));
            Assert.True(staffs.Any(s => s.Id == staffIds[1] && s.IsAssigned == false));
            Assert.True(staffs.Any(s => s.Id == staffIds[2] && s.IsAssigned == false));
        }


        private void Init_First_Staff_And_JobData()
        {
            // first cso staff id 1
            _repositoryService.Add(Staff.Create("056bdf95-c2f0-4fa3-9a7e-b65f0cb7b215", "Fullname CSO Staff", "CSOStaffAccount", "csoemail@gmail.com"));
            _repositoryService.SaveChanges();

            _staffService.CreateStaff(new CreateStaffVM()
            {
                FullName = "Fullname Staff 2",
                Account = "Staff2",
                Email = "staff2@gmail.com"
            });
            _staffService.CreateStaff(new CreateStaffVM()
            {
                FullName = "Fullname Staff 3",
                Account = "Staff3",
                Email = "staff3@gmail.com"
            });

            // first job id 1 with ProductLevel 1
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

            var createJobVM2 = new CreateJobVM()
            {
                Date = DateTime.Now,
                LocationId = 2,
                CSOStaffId = 1,
                JobName = "Retouching 2",
                Instruction = "This is a high end property, can you work your magic on these please",
                InputInfo = "{imageNumber: 300}",
                InputNumber = 30,
                DeliverTypeId = 1,
                ProductLevelId = 1,
                Deadline = DateTime.Now.AddDays(1),
                AppId = 2
            };
            _jobService.CreateJob(createJobVM);
            _jobService.CreateJob(createJobVM2);

            var productLevels = _repositoryService.List<ProductLevel>(new int[] { 1, 2, 3, 4 });

            // steps 1, 2, 3 has ProductLevel 1, 2, 3
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
