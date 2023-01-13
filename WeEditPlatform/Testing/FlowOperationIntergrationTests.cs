using Application.Commands;
using Application.Models;
using Application.Services;
using Domain;
using Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Testing
{
    public class FlowOperationIntergrationTests : TestBase
    {
        private IOperationService _operationService;
        private IFlowService _flowService;

        [OneTimeSetUp]
        public void Init()
        {
            _operationService = _serviceProvider.GetService<IOperationService>();
            _flowService = _serviceProvider.GetService<IFlowService>();
        }

        [Test]
        [TestCase("flow 1", "flow auto assign action instant", FlowType.Instant, FlowStatus.On,

            "assignAction1", "assign job to taff", OperationType.Action, "AssignAction",

            "{\"IsGroupMatching\": false, \"IsProductLevelMatching\" : true}",

            "select * from JobSteps where Status is null || Status = 0",

            "jobstep.isExpried",

            "select b.* from (select * from StaffRoles where RoleId in (1, 2, 3)) as a join (select * from Staffs where DateDeleted = 0 && IsAssigned = 0 && (CurrentShiftId in(1,2,3) )) as b on a.StaffId = b.Id")]
        public void Add_Flow_Operation_With_Setting_Then_Invoke(
            string flowName, string flowDescription, FlowType flowType, FlowStatus flowStatus,
            string operationName, string operationDescription, OperationType operationType, string executionName,
            string matchingAssignSetting,
            string rawSqlFilterJobStep,
            string validJobStepIsExpriedMethod,
            string rawSqlFilterStaff
            )
        {
            var settings = new Setting[]
            {
                new Setting(){ Key = ClassExtension.GetMemberName((AssignActionCommand c) => c.MatchingAssignSetting),
                 Value = matchingAssignSetting },
                new Setting() { Key = ClassExtension.GetMemberName((AssignActionCommand c) => c.RawSqlFilterJobStep),
                Value = rawSqlFilterJobStep },
                 new Setting() { Key = ClassExtension.GetMemberName((AssignActionCommand c) => c.ValidJobStepIsExpriedMethod),
                Value = ""},
                  new Setting() { Key = ClassExtension.GetMemberName((AssignActionCommand c) => c.RawSqlFilterStaff),
                Value = rawSqlFilterStaff},
            };

            var createFlowVM = new CreateFlowVM()
            {
                Name = flowName,
                Description = flowDescription,
                Status = flowStatus,
                Type = flowType,
            };

            var flowCreated = _flowService.CreateFlow(createFlowVM);

            Assert.NotNull(flowCreated);
            Assert.NotNull(flowCreated.Id);

            var createOperationVM = new CreateOperationVM()
            {
                FlowId = flowCreated.Id,
                Type = operationType,
                Name = operationName,
                Description = operationDescription,
                ExecutionName = executionName,
                Settings = settings
            };

            var operationCreated = _operationService.CreateOperation(createOperationVM);

            Assert.NotNull(operationCreated);
            Assert.NotNull(operationCreated.Uid);

            _operationService.Invoke(operationCreated.Id);
        }

        [Test]
        [TestCase(1)]
        public void Run_Flow_Success(int flowId)
        {
            var invokeResult = _flowService.RunFlow(flowId).Result;

            Assert.NotNull(invokeResult);
            Assert.IsTrue(invokeResult.Success);
        }

    }
}
